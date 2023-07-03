using WebApiSample.Models;
using WebApiSample.Infrastructure;

// LISTED 27_6_2023 16:51

public class calc
{
    // Con UnitOfWork accedo a todos los repositorios.
    private IUnitOfWork _unitOfWork;
    private IEstimateService _estService;

    public string  haltError;
    public string  warnings;

    public calc(IUnitOfWork unitOfWork, IEstimateService estService)
    {
        _unitOfWork=unitOfWork;
        _estService=estService;
    }


// Ejecuta el batch de calculos, de izq a derecha conforme el libro de XLS
// Segun "Presupuestador Argentina, libro N - Duchas Escocesas"
// MODIFICADO 3_7_2023 ==>
// Calcbatch antes trabajaba sobre registros ingresados a la base.
// Esto debe hacerse en RAM, para evitar grabar presupuestos ANTES de saber si los calculos
// fallan. Una vez que los calclos son correctos, el etimateV2 se pasa a EstimateDB 
// y se guerda en la basa. El estimateV2 es en verdad lo que se envia como JSON se salida
    public async Task<EstimateV2> calcBatch(EstimateDB miEst)
    {

        EstimateDB myEstDB=new EstimateDB(); 
        dbutils dbhelper=new dbutils(_unitOfWork);

        // Comentado 3_7_2023
        /*if(miEst.estHeaderDB.EstNumber>0)
        {
            myEstDB=await dbhelper.getEstimateLastVers(miEst.estHeaderDB.EstNumber);
            if(myEstDB==null)
            {
                haltError=$"NO SE ENCONTRO HEADER ! estNum:{miEst.estHeaderDB.EstNumber}";
                return null;
            }
        }*/
        //else
        //{
            myEstDB=miEst;
        //}

        // El objeto Estimate que se definio. 
        EstimateV2 myEstV2=new EstimateV2();
        

        // Expando el EstimateDB a un EstimateV2
        myEstV2=dbhelper.transferDataFromDBType(myEstDB);

        // Levanto todas las constantes.
        myEstV2= await _estService.loadConstants(myEstV2);
        if(myEstV2==null)
        {
            haltError="Tabla Constantes no accesible";
            return null;
        }

        // Hago algunas cuentas.
        // Calculo el peso total por articulo
        // COL J
        myEstV2=_estService.CalcPesoTotal(myEstV2);
        if(myEstV2==null)
        {
            haltError=_estService.getLastError();
            return null;
        }
        // COL K
        myEstV2=_estService.CalcCbmTotal(myEstV2);
        if(myEstV2==null)
        {
            haltError=_estService.getLastError();
            return null;
        }
        // CELDA K43
        myEstV2=_estService.CalcCbmGrandTotal(myEstV2);
        // CELDA C10
        myEstV2=await _estService.CalcularCantContenedores(myEstV2);
        if(myEstV2==null)
        {
            haltError="La tabla de contenedores no es accesible o el volumen del contenedor es 0";
            return null;
        }
        // COL L. Calculo el fob total por articulo
        myEstV2=_estService.CalcFobTotal(myEstV2);
        // CELDA L43. Sumo todos los fob totales. Sumatoria de L15-L41 que se copia en celda C3
        myEstV2.FobGrandTotal=_estService.sumFobTotal(myEstV2);
        // CELDA C5 que es funcion del valor FOB
        myEstV2=_estService.CalcSeguroTotal(myEstV2);
        // CELDA C4. Traigo la tarifa del flete desde BASE_TARIFAS por fowarder y tipo cont
        //TarifasFwdCont myTar=await _unitOfWork.TarifasFwdContenedores.GetByFwdContTypeAsync(myEstV2.FreightFwd,myEstV2.FreightType);
        // De la consulta me quedo con el valor del flete (se usa 60%)
        //myEstV2.FleteTotal=await _estService.lookUpTarifaFleteCont(myEstV2);
        myEstV2=await _estService.CalcFleteTotal(myEstV2);
        if(myEstV2==null)
        {
            haltError="Tarifas Flete Fowarder Inaccesible";
            return null;
        }
        // COL M. Calcula el flete ponderado a cada articulo del detalle.
        myEstV2=_estService.CalcFleteTotalByProd(myEstV2);
        if(myEstV2==null)
        {
            haltError="ATENCION: CalcFleteTotalByProd. FOB GRAND TOTAL ES 0. DIV 0 !";
            return null;
        }
        // COL N. Calcula el seguro ponderado a cada articulo del detalle 
        myEstV2=_estService.CalcSeguro(myEstV2);
        if(myEstV2==null)
        {
            haltError="ATENCION: CalcSeguro. FOB GRAND TOTAL es 0. DOV 0 !";
        }
        // COL O. Calcula el CIF que solo depende de los datos ya calculados previamente (COL L, N y M)
        myEstV2=_estService.CalcCif(myEstV2);
        // CELDA =43
        myEstV2=_estService.CalcCifTotal(myEstV2);
        // COL R (COL O y COL Q no estan en uso)
        myEstV2=_estService.CalcAjusteIncDec(myEstV2);
        // COL S, COL U, COLY, COL AA 
        // Evito consultar la base de NCM una vez por cada factor necesario. Se que son 4 los factores.
        // Los traigo en una sola consulta (una consulta x item)
        myEstV2=await _estService.search_NCM_DATA(myEstV2);
        if(myEstV2==null)
        {   
            haltError=_estService.getLastError();
            return null;
        }
        //myEstV2=await _estService.searchNcmDie(myEstV2);
        // COL T
        myEstV2=_estService.CalcDerechos(myEstV2);
        // COL U
        //myEstV2=await _estService.resolveNcmTe(myEstV2);
        // COL V
        myEstV2=_estService.CalcTasaEstad061(myEstV2);
        // COL X
        myEstV2=_estService.CalcBaseGcias(myEstV2); 
        // COL Y
        //myEstV2=await _estService.searchIva(myEstV2); 
        // COL Z
        myEstV2=_estService.CalcIVA415(myEstV2);
        // COL AA
        //myEstV2= await _estService.searchIvaAdic(myEstV2);
        // COL AB
        myEstV2=_estService.CalcIVA_ad_Gcias(myEstV2);
        // COL AC
        myEstV2=_estService.CalcImpGcias424(myEstV2);
        // COL AD
        myEstV2=await _estService.CalcIIBB900(myEstV2); 
        // COL AE
        myEstV2=_estService.CalcPrecioUnitUSS(myEstV2);
        if(myEstV2==null)
        {
            haltError=_estService.getLastError();
            return null;
        }
        // COL AF
        myEstV2=_estService.CalcPagado(myEstV2);
        // CELDA AF43
        myEstV2=_estService.CalcPagadoTot(myEstV2);
        // AH
        myEstV2=_estService.CalcFactorProdTotal(myEstV2);
        if(myEstV2==null)
        {
            haltError=_estService.getLastError();
            return null;
        }
        // Proceso todos los gastos proyectados.
        myEstV2.ExtraGastosLocProyectado=await _estService.calcularGastosProyecto(myEstV2);
        if(myEstV2.ExtraGastosLocProyectado<0)
        {
            haltError=_estService.getLastError();
            return null;
        }
        // AI
        myEstV2=_estService.CalcExtraGastoLocProyecto(myEstV2);
        //AJ
        myEstV2=_estService.CalcExtraGastoProyectoUSS(myEstV2);
        //AK
        myEstV2=_estService.CalcExtraGastoProyectoUnitUSS(myEstV2);
        if(myEstV2==null)
        {
            haltError=_estService.getLastError();
            return null;
        }
        //AL
        myEstV2=_estService.CalcOverhead(myEstV2);
        if(myEstV2==null)
        {
            haltError=_estService.getLastError();
            return null;
        }
        //AM
        myEstV2=_estService.CalcCostoUnitarioUSS(myEstV2);
        //AN
        myEstV2=_estService.CalcCostoUnitario(myEstV2);
        // LISTED 22_6_2023 !!!. Todas las cuentas OK-
        return myEstV2;
    }












public async Task<EstimateV2> calcReclaim(EstimateDB miEst)
    {

        EstimateDB myEstDB=new EstimateDB(); 
        dbutils dbhelper=new dbutils(_unitOfWork);


        myEstDB=miEst;

        // El objeto Estimate que se definio. 
        EstimateV2 myEstV2=new EstimateV2();
        

        // Expando el EstimateDB a un EstimateV2
        myEstV2=dbhelper.transferDataFromDBType(myEstDB);

        // Hago algunas cuentas.
        // Calculo el peso total por articulo
        // COL J
        myEstV2=_estService.CalcPesoTotal(myEstV2);
        if(myEstV2==null)
        {
            haltError=_estService.getLastError();
            return null;
        }
        // COL K
        myEstV2=_estService.CalcCbmTotal(myEstV2);
        if(myEstV2==null)
        {
            haltError=_estService.getLastError();
            return null;
        }

        // COL L. Calculo el fob total por articulo
        myEstV2=_estService.CalcFobTotal(myEstV2);

        // COL M. Calcula el flete ponderado a cada articulo del detalle.
        myEstV2=_estService.CalcFleteTotalByProd(myEstV2);
        if(myEstV2==null)
        {
            haltError="ATENCION: CalcFleteTotalByProd. FOB GRAND TOTAL ES 0. DIV 0 !";
            return null;
        }
        // COL N. Calcula el seguro ponderado a cada articulo del detalle 
        myEstV2=_estService.CalcSeguro(myEstV2);
        if(myEstV2==null)
        {
            haltError="ATENCION: CalcSeguro. FOB GRAND TOTAL es 0. DOV 0 !";
        }
        // COL O. Calcula el CIF que solo depende de los datos ya calculados previamente (COL L, N y M)
        myEstV2=_estService.CalcCif(myEstV2);
        // COL R (COL O y COL Q no estan en uso)
        myEstV2=_estService.CalcAjusteIncDec(myEstV2);
        // COL S, COL U, COLY, COL AA 
        // Evito consultar la base de NCM una vez por cada factor necesario. Se que son 4 los factores.
        // Los traigo en una sola consulta (una consulta x item)
        myEstV2=await _estService.search_NCM_DATA(myEstV2);
        if(myEstV2==null)
        {   
            haltError=_estService.getLastError();
            return null;
        }
        //myEstV2=await _estService.searchNcmDie(myEstV2);
        // COL T
        myEstV2=_estService.CalcDerechos(myEstV2);
        // COL U
        //myEstV2=await _estService.resolveNcmTe(myEstV2);
        // COL V
        myEstV2=_estService.CalcTasaEstad061(myEstV2);
        // COL X
        myEstV2=_estService.CalcBaseGcias(myEstV2); 
        // COL Y
        //myEstV2=await _estService.searchIva(myEstV2); 
        // COL Z
        myEstV2=_estService.CalcIVA415(myEstV2);
        // COL AA
        //myEstV2= await _estService.searchIvaAdic(myEstV2);
        // COL AB
        myEstV2=_estService.CalcIVA_ad_Gcias(myEstV2);
        // COL AC
        myEstV2=_estService.CalcImpGcias424(myEstV2);
        // COL AD
        myEstV2=await _estService.CalcIIBB900(myEstV2); 
        // COL AE
        myEstV2=_estService.CalcPrecioUnitUSS(myEstV2);
        if(myEstV2==null)
        {
            haltError=_estService.getLastError();
            return null;
        }
        // COL AF
        myEstV2=_estService.CalcPagado(myEstV2);
        // AH
        myEstV2=_estService.CalcFactorProdTotal(myEstV2);
        if(myEstV2==null)
        {
            haltError=_estService.getLastError();
            return null;
        }

        // AI
        myEstV2=_estService.CalcExtraGastoLocProyecto(myEstV2);
        //AJ
        myEstV2=_estService.CalcExtraGastoProyectoUSS(myEstV2);
        //AK
        myEstV2=_estService.CalcExtraGastoProyectoUnitUSS(myEstV2);
        if(myEstV2==null)
        {
            haltError=_estService.getLastError();
            return null;
        }
        //AL
        myEstV2=_estService.CalcOverhead(myEstV2);
        if(myEstV2==null)
        {
            haltError=_estService.getLastError();
            return null;
        }
        //AM
        myEstV2=_estService.CalcCostoUnitarioUSS(myEstV2);
        //AN
        myEstV2=_estService.CalcCostoUnitario(myEstV2);
        // LISTED 22_6_2023 !!!. Todas las cuentas OK-
        return myEstV2;
    }





















// Este metodo es similar al anterior salvo que tiene opcion de no volver a buscar a base los mismo valores una y otra vez.
// Es consumido por el metodo aCalc, que "despeja" un valor de una columna a la derecha fijado un valor a la izquierda
// Ejemplo, determina el valor fobunit para alcanzar un valor de aduanaDivisa dado.
    public async Task<EstimateV2> calcOnce(EstimateV2 myEstV2, Contenedor myCont,double tarifaFleteCont,NCM myNCM,double sumarIIBB,bool once)
    {
        EstimateDB myEstDB=new EstimateDB(); 
        dbutils dbhelper=new dbutils(_unitOfWork);

        // COL J
        myEstV2.EstDetails[0].PesoTot=_estService._estDetServices.CalcPesoTotal(myEstV2.EstDetails[0]); 
        if(myEstV2.EstDetails[0].PesoTot<0)
        {
            return null;
        }
        // COL K
        myEstV2.EstDetails[0].CbmTot=_estService._estDetServices.CalcCbmTotal(myEstV2.EstDetails[0]);  
        if(myEstV2.EstDetails[0].CbmTot<0) 
        {
            return null;
        }
        myEstV2.CbmGrandTot=myEstV2.EstDetails[0].CbmTot;
        // CELDA C10
        if(myCont==null)        
        {
            return null;
        }
        if(myCont.volume>0)
        {
            myEstV2.CantidadContenedores=myEstV2.CbmGrandTot/myCont.volume;
        }
        else
        {
            return null;
        }
        // FIN CELDA C10.
        // COL L
        myEstV2.EstDetails[0].Fob=_estService._estDetServices.CalcFob(myEstV2.EstDetails[0]);    
        // CELDA L43
        myEstV2.FobGrandTotal=myEstV2.EstDetails[0].Fob;
        // CELDA C5
        myEstV2.Seguro=(myEstV2.constantes.CNST_SEGURO_PORCT/100)*myEstV2.FobGrandTotal;
        // CELDA C4
        myEstV2.FleteTotal=tarifaFleteCont*myEstV2.CantidadContenedores;
        // COL M
        myEstV2.EstDetails[0].Flete=_estService._estDetServices.CalcFlete(myEstV2.EstDetails[0],myEstV2.FleteTotal,myEstV2.FobGrandTotal);
        if(myEstV2.EstDetails[0].Flete<0)
        {
            return null;
        }
        // COL N
        myEstV2.EstDetails[0].Seguro=_estService._estDetServices.CalcSeguro(myEstV2.EstDetails[0],myEstV2.Seguro,myEstV2.FobGrandTotal);   
        if(myEstV2.EstDetails[0].Seguro<0)
        {
            return null;
        }
        // COL O
        myEstV2.EstDetails[0].Cif=_estService._estDetServices.CalcCif(myEstV2.EstDetails[0]);
        // CELDA O43
        myEstV2.CifTot=myEstV2.EstDetails[0].Cif;
        // COL R
        myEstV2.EstDetails[0].valAduanaDivisa=_estService._estDetServices.CalcValorEnAduanaDivisa(myEstV2.EstDetails[0]);
        // COL S, U, Y, AA
        myEstV2.EstDetails[0].Die=myNCM.die/100.0;
        myEstV2.EstDetails[0].Te=myNCM.te/100.0;
        myEstV2.EstDetails[0].IVA=myNCM.iva/100.0;
        myEstV2.EstDetails[0].IVA_ad=myNCM.iva_ad/100.0; 
        // COL T
        myEstV2.EstDetails[0].Derechos=_estService._estDetServices.CalcDerechos(myEstV2.EstDetails[0]);
        // COL V
        myEstV2.EstDetails[0].TasaEstad061=_estService._estDetServices.CalcTasaEstad061(myEstV2.EstDetails[0]);
        // COL X
        myEstV2.EstDetails[0].BaseIvaGcias=_estService._estDetServices.CalcBaseIvaGcias(myEstV2.EstDetails[0]);
        // COL Z
        myEstV2.EstDetails[0].IVA415=_estService._estDetServices.CalcIVA415(myEstV2.EstDetails[0]);
        // COL AB
        myEstV2.EstDetails[0].IVA_ad_gcias=_estService._estDetServices.CalcIvaAdic(myEstV2.EstDetails[0],myEstV2.IvaExcento);
        // COL AC
        myEstV2.EstDetails[0].ImpGcias424=_estService._estDetServices.CalcImpGcias424(myEstV2.EstDetails[0]);
        // COL AD
        myEstV2.EstDetails[0].IIBB=sumarIIBB;
        // COL AE
        myEstV2.EstDetails[0].PrecioUnitUSS=_estService._estDetServices.CalcPrecioUnitUSS(myEstV2.EstDetails[0]);
        if(myEstV2.EstDetails[0].PrecioUnitUSS<0)
        {
            return null;
        }
        // COL AF
        myEstV2.EstDetails[0].Pagado=_estService._estDetServices.CalcPagado(myEstV2.EstDetails[0]);
        // CELDA AF43
        myEstV2.Pagado=myEstV2.EstDetails[0].Pagado+myEstV2.constantes.CNST_ARANCEL_SIM;
        // COL AH
        myEstV2.EstDetails[0].FactorProd=_estService._estDetServices.CalcFactorProducto(myEstV2.EstDetails[0],myEstV2.FobGrandTotal); 
        if( myEstV2.EstDetails[0].FactorProd<0)
        {
            return null;
        }
        // TABLA DE GASTOS PROYECTADOS: SACO EL TOTAL EN PESOS.
        // Nota, no lo puedo llamar afuera de esta funcion por que los calculos solo
        // podran hacrse cuando los datos necesarios hayan sido populados
        // Uso el flag once para que solo se ejecute una vez, en el primer llamado.
        if(once)
        {
            myEstV2.ExtraGastosLocProyectado=await _estService.calcularGastosProyecto(myEstV2);
            if(myEstV2.ExtraGastosLocProyectado<0)
            {
                haltError=_estService.getLastError();
                return null;
            }
        }
        // COL AI
        myEstV2.EstDetails[0].ExtraGastoLocProy=_estService._estDetServices.CalcGastosProyPond(myEstV2.EstDetails[0],myEstV2.ExtraGastosLocProyectado);       
        // COL AJ
        myEstV2.EstDetails[0].ExtraGastoLocProyUSS=_estService._estDetServices.CalcGastosProyPondUSS(myEstV2.EstDetails[0],myEstV2.DolarBillete);       
        // COL AK
        myEstV2.EstDetails[0].ExtraGastoLocProyUnitUSS=_estService._estDetServices.CalcGastosProyPorUnidUSS(myEstV2.EstDetails[0]); 
        if(myEstV2.EstDetails[0].ExtraGastoLocProyUnitUSS<0)
        {
            return null;
        }
        // COL AL
        myEstV2.EstDetails[0].OverHead=_estService._estDetServices.CalcOverHeadUnitUSS(myEstV2.EstDetails[0]);  
        if(myEstV2.EstDetails[0].OverHead<0)
        {
            return null;
        }
        // COL AM
        myEstV2.EstDetails[0].CostoUnitEstimadoUSS=_estService._estDetServices.CalcCostoUnitUSS(myEstV2.EstDetails[0]); 
        // COL AN
        myEstV2.EstDetails[0].CostoUnitEstimado=_estService._estDetServices.CalcCostoUnit(myEstV2.EstDetails[0],myEstV2.DolarBillete); 
        

        return myEstV2;
        return myEstV2;
    }


    // aCalc: Hace una corrida de calculo similar a la que se haria para hacer un estimado, pero permite elegir un paramtro a variar
    // (propertyNameIn) entorno a un valor estimado (adjValueIn) hasta que un segundo paramtro (propertyNameout) alcance un valor deseado 
    // (adjValueout) pasado como paramtro.
    // Esto permitiria fija un valor deseado en cualquier columna "A" y ver que valor necesario necesario en otra columna "B" para 
    // alcanzarlo. Permite que entrada y salida sean cualquier columna (en teoria).
    // La clara limitacion es que esto no es un despeje, sino una aproximacion sucesiva.
    // Para evitar agregar lentitud, los items que sean de consulta a la base se haran por una unica vez, dado que estos no variaran a lo 
    // largo de las iteraciones.
    // El ajuste es tipo PD, donde el step es ajustado segun la diferencia entre el traget y el iterado.
    // LISTED 28/6/2023 12:55 
    public async Task<EstimateV2>aCalc(int estNumber,double adjValueIn, string propertyNameIn, double adjValueOut, string propertyNameOut)
    {
        EstimateDB myEstDB=new EstimateDB(); 
        dbutils dbhelper=new dbutils(_unitOfWork);
        myEstDB=await dbhelper.getEstimateLastVers(estNumber);
        double step=10;
        double sentido=1;
        double tmpOut;
        double cntSmallStepDown=0;
        double cntSmallStepUp=0;
        int pass=0;
        int speed=1;
        double valueIniCopy=adjValueIn;


        int cnt1=0;
        int cnt2=0;
        int cnt3=0;
        int cnt4=0;
        int cnt5=0;
        int cnt6=0;

        // El objeto Estimate que se definio. 
        EstimateV2 myEstV2=new EstimateV2();
        // Levanto todas las constantes.
        myEstV2= await _estService.loadConstants(myEstV2);
        if(myEstV2==null)
        {
            haltError="Tabla Constantes no accesible";
            return null;
        }
        // Expando el EstimateDB a un EstimateV2
        myEstV2=dbhelper.transferDataFromDBType(myEstDB);


        var adjPropIn=myEstV2.EstDetails[0].GetType().GetProperty(propertyNameIn);
        if(adjPropIn==null)
        {
            return null;
        }
        var adjPropOut=myEstV2.EstDetails[0].GetType().GetProperty(propertyNameOut);
        if(adjPropOut==null)
        {
            return null;
        }

        // Cargo el valor "aproximado" de la propiedad a determinar.
        if(adjPropIn!=null)
        {
            // Inicio el calculo completo con todas las consultas a la base.
            // luego el segundo parametro sera false dado que los datos de los demas libros no cambiaran duranrte la aproximacion
            adjPropIn.SetValue(myEstV2.EstDetails[0],adjValueIn);
        }
        else
        {
            return null;
        }

// FIN TEST - DESCOMENTAR 

// TEST - COMENTAR
/*public async Task<EstimateV2>calcOnceTest(EstimateDB myEstDB)
{ 
        dbutils dbhelper=new dbutils(_unitOfWork);
        EstimateV2 myEstV2=new EstimateV2();
        // Levanto todas las constantes.
        myEstV2= await _estService.loadConstants(myEstV2);
        if(myEstV2==null)
        {
            haltError="Tabla Constantes no accesible";
            return null;
        }
        myEstV2=dbhelper.transferDataFromDBType(myEstDB);
// FIN TEST - COMENTAR */

        Contenedor myCont=new Contenedor();
        myCont=await _unitOfWork.Contenedores.GetByTipoContAsync(myEstV2.FreightType);

        double tarifaFleteCont;
        tarifaFleteCont=await _estService.lookUpTarifaFleteCont(myEstV2);

        NCM myNCM=new NCM();
        myNCM=await _estService._estDetServices.lookUp_NCM_Data(myEstV2.EstDetails[0]); 

        double sumaIIBB;
        sumaIIBB=await _estService._estDetServices.CalcIIBB(myEstV2.EstDetails[0]);


        // Hago los calculos forzando (por unica vez) que todos lo datos que vienen de otras tablas sean consultados.
        myEstV2=await calcOnce(myEstV2,myCont,tarifaFleteCont,myNCM,sumaIIBB,true);


    
        tmpOut=(double)adjPropOut.GetValue(myEstV2.EstDetails[0]);

        // Inicializo la cantidad de pasadas.
        pass=0;

        // Mientras no haya iterado mas de 200 veces ... do
        while(pass<300)
        {

            // Detemrino el step y el sentido
            // La cantidada resultante excede el valor esperado. El sentido del ajuste es negativo.
            // A medida que decrece la diferencia entre el valor deseado y el iterado, se reduce el step
            // Si la diferencia es mas grande que 300, el step va aumentando de a 15 cuentas.
            // Esto es identico para el otro sentido (el else).
            // Control PD
            if(tmpOut>adjValueOut)
            {
                sentido=-1;
                // Muy fuera del valor esperado .... incremento el step en cada iteracion.
                // Es el inico rango con step variable
                if((tmpOut-adjValueOut)>(adjValueOut/3))
                {
                    step=valueIniCopy/12;
                }
                else if((tmpOut-adjValueOut)>(adjValueOut/10))
                {
                    step=valueIniCopy/15;
                }
                else if((tmpOut-adjValueOut)>(adjValueOut/40))
                {
                    step=valueIniCopy/30;
                }
                else if((tmpOut-adjValueOut)>(adjValueOut/100))
                {
                    step=valueIniCopy/120;
                }
                else if((tmpOut-adjValueOut)>(adjValueOut/400))
                {
                    step=valueIniCopy/300;
                }
                // Ajuste fino. Aqui es posible que comience a oscilar entre correcion ascendente y descendente.
                // La oscilacion significa que ya se encontro el valor deseado entorno a +/-1 cuenta.
                // En el paso de ajuste fino, sea positivo y/o negativo se instalaron sendos contadores que registran
                // la cantidad de pasadas. Si ambos contadores estan en 2 o mas significa que el valor esta llendo entre
                // + y - una cuenta .... 
                else 
                {
                    step=valueIniCopy/1024;
                    cntSmallStepDown++;
                }
            }
            else
            // La cantidad resultante es inferior al valor esperado. El ajuste es positivo.
            // A medida que 
            {
                sentido=1;
                // Muy fuera del valor esperado .... incremento el step en cada iteracion.
                // Es el inico rango con step variable
                if((adjValueOut-tmpOut)>(adjValueOut/3))
                {
                    step=valueIniCopy/12;
                    cnt1++;
                }
                else if((adjValueOut-tmpOut)>(adjValueOut/10))
                {
                    step=valueIniCopy/15;
                    cnt2++;
                }
                else if((adjValueOut-tmpOut)>(adjValueOut/40))
                {
                    step=valueIniCopy/30;
                    cnt3++;
                }
                else if((adjValueOut-tmpOut)>(adjValueOut/100))
                {
                    step=valueIniCopy/120;
                    cnt4++;
                }
                else if((adjValueOut-tmpOut)>(adjValueOut/600))
                {
                    step=valueIniCopy/400;
                    cnt5++;
                }
                // Ajuste fino. Aqui es posible que comience a oscilar entre correcion ascendente y descendente.
                // La oscilacion significa que ya se encontro el valor deseado entorno a +/-1 cuenta.
                // En el paso de ajuste fino, sea positivo y/o negativo se instalaron sendos contadores que registran
                // la cantidad de pasadas. Si ambos contadores estan en 2 o mas significa que el valor esta llendo entre
                // + y - una cuenta .... 
                else 
                {
                    step=valueIniCopy/1024;
                    cntSmallStepUp++;
                    cnt6++;
                }
            }
            // Estamos en torno a +/- 0.1 cuenta ?
            if(cntSmallStepDown>1 && cntSmallStepUp>1)
            {
                break; // SI, corto la iteracion
            }

            // Analizo la velocidad de convergencia de in vs out        
            double delta=valueIniCopy-adjValueIn;
            if(delta<0)
            {   // Si es negativo lo hago positivo. ME interesa solo el valor absoluto.
                delta*=-1;
            }

            // Si luego de 10 iteraciones el delta es menor al valor/50 aumento la velocidad x10.
            if(((delta*50)<valueIniCopy) && pass==10)
            {
                speed*=10;
            }
            // Si luego de 20 iteraciones el delta persiste en el valor/50, aumento la velocidad otras 5x
            if(((delta*50)<valueIniCopy) && pass==20)
            {
                speed*=5;
            }

            // Ajusto el valor entrante (el valor a "DESPEJAR")
            adjValueIn+=(step*sentido*speed);
            // Lo cargo en el objeto.
            adjPropIn.SetValue(myEstV2.EstDetails[0],adjValueIn);
            // Corro los calculos de nuevo
            calcOnce(myEstV2,myCont,tarifaFleteCont,myNCM,sumaIIBB,true);
            // Me fijo el valor saliente (este vlaor lo fijo el usuario como DATO)
            tmpOut=(double)adjPropOut.GetValue(myEstV2.EstDetails[0]);


            // cuento la cantidad de iteraciones.
            pass++;
        }

        return myEstV2;
    }
  
}



