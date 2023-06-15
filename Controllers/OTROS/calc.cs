using WebApiSample.Models;
using WebApiSample.Infrastructure;

// PRE LISTED 15_6_2023 15:31

public class calc
{
    // Con UnitOfWork accedo a todos los repositorios.
    private IUnitOfWork _unitOfWork;
    private IEstimateService _estService;

    public calc(IUnitOfWork unitOfWork, IEstimateService estService)
    {
        _unitOfWork=unitOfWork;
        _estService=estService;
    }


// Ejecuta el batch de calculos, de izq a derecha conforme el libro de XLS
// Segun "Presupuestador Argentina, libro N - Duchas Escocesas"

// Me dan como parametro el numero de presupuesto.
    public async Task<EstimateV2> calcBatch(int estNumber)
    {

        EstimateDB myEstDB=new EstimateDB(); 
        dbutils dbhelper=new dbutils(_unitOfWork);
        myEstDB=await dbhelper.getEstimateLastVers(estNumber);


        // El objeto Estimate que se definio. 
        EstimateV2 myEstV2=new EstimateV2();
        // Levanto todas las constantes.
        myEstV2= await _estService.loadConstants(myEstV2);

        // Expando el EstimateDB a un EstimateV2
        myEstV2=dbhelper.transferDataFromDBType(myEstDB);

        // Hago algunas cuentas.
        // Calculo el peso total por articulo
        // COL J
        myEstV2=_estService.CalcPesoTotal(myEstV2);
        // COL K
        myEstV2=_estService.CalcCbmTotal(myEstV2);
        // COL L. Calculo el fob total por articulo
        myEstV2=_estService.CalcFobTotal(myEstV2);
        // CELDA L43. Sumo todos los fob totales. Sumatoria de L15-L41 que se copia en celda C3
        myEstV2.FobGrandTotal=_estService.sumFobTotal(myEstV2);
        // CELDA C4. Traigo la tarifa del flete desde BASE_TARIFAS por fowarder y tipo cont
        TarifasFwdCont myTar=await _unitOfWork.TarifasFwdContenedores.GetByFwdContTypeAsync(myEstV2.FreightFwd,myEstV2.FreightType);
        // De la consulta me quedo con el valor del flete (se usa 60%)
        myEstV2.FleteTotal=await _estService.lookUpTarifaFleteCont(myEstV2);
        // COL M. Calcula el flete ponderado a cada articulo del detalle.
        myEstV2=_estService.CalcFleteTotal(myEstV2);
        // COL N. Calcula el seguro ponderado a cada articulo del detalle 
        myEstV2=_estService.CalcSeguro(myEstV2);
        // COL O. Calcula el CIF que solo depende de los datos ya calculados previamente (COL L, N y M)
        myEstV2=_estService.CalcCif(myEstV2);
        // COL R (COL O y COL Q no estan en uso)
        myEstV2=_estService.CalcAjusteIncDec(myEstV2);
        // COL S (die segun NCM)
        myEstV2=await _estService.searchNcmDie(myEstV2);
        // COL T
        myEstV2=_estService.CalcDerechos(myEstV2);
        // COL U
        myEstV2=await _estService.resolveNcmTe(myEstV2);
        // COL V
        myEstV2=_estService.CalcTasaEstad061(myEstV2);
        // COL X
        myEstV2=_estService.CalcBaseGcias(myEstV2); 
        // COL Y
        myEstV2=await _estService.searchIva(myEstV2);
        // COL Z
        myEstV2=_estService.CalcIVA415(myEstV2);
        // COL AA
        myEstV2= await _estService.searchIvaAdic(myEstV2);
        // COL AB
        myEstV2=_estService.CalcIVA_ad_Gcias(myEstV2);
        // COL AC
        myEstV2=_estService.CalcImpGcias424(myEstV2);
        // COL AD
        myEstV2=await _estService.CalcIIBB900(myEstV2);
        // COL AE
        myEstV2=_estService.CalcPrecioUnitUSS(myEstV2);
        // COL AF
        myEstV2=_estService.CalcPagado(myEstV2);
        // AH
        myEstV2=_estService.CalcFactorProdTotal(myEstV2);
        // Proceso todos los gastos proyectados.
        myEstV2.ExtraGastosLocProyectado=await _estService.calcularGastosProyecto(myEstV2);
        // AI
        myEstV2=_estService.CalcExtraGastoLocProyecto(myEstV2);
        //AJ
        myEstV2=_estService.CalcExtraGastoProyectoUSS(myEstV2);
        //AK
        myEstV2=_estService.CalcExtraGastoProyectoUnitUSS(myEstV2);
        //AL
        myEstV2=_estService.CalcOverhead(myEstV2);
        //AM
        myEstV2=_estService.CalcCostoUnitarioUSS(myEstV2);
        //AN
        myEstV2=_estService.CalcCostoUnitario(myEstV2);

        return myEstV2;
    }
// Este metodo es similar al anterior salvo que tiene opcion de no volver a buscar a base los mismo valores una y otra vez.
// Es consumido por el metodo aCalc, que "despeja" un valor de una columna a la derecha fijado un valor a la izquierda
// Ejemplo, determina el valor fobunit para alcanzar un valor de aduanaDivisa dado.
    public async Task<EstimateV2> calcOnce(EstimateV2 myEstV2,bool once)
    {
        myEstV2=_estService.CalcPesoTotal(myEstV2);
        myEstV2=_estService.CalcCbmTotal(myEstV2);
        myEstV2=_estService.CalcFobTotal(myEstV2);
        myEstV2.FobGrandTotal=_estService.sumFobTotal(myEstV2);
        if(once)
        {
            TarifasFwdCont myTar=await _unitOfWork.TarifasFwdContenedores.GetByFwdContTypeAsync(myEstV2.FreightFwd,myEstV2.FreightType);
            myEstV2.FleteTotal=await _estService.lookUpTarifaFleteCont(myEstV2);
        }
        myEstV2=_estService.CalcFleteTotal(myEstV2);
        myEstV2=_estService.CalcSeguro(myEstV2);
        myEstV2=_estService.CalcFactorProdTotal(myEstV2);
        myEstV2=_estService.CalcCif(myEstV2);
        myEstV2=_estService.CalcAjusteIncDec(myEstV2);
        if(once)
        {
            myEstV2=await _estService.searchNcmDie(myEstV2);
        }
        myEstV2=_estService.CalcDerechos(myEstV2);
        if(once)
        {
            myEstV2=await _estService.resolveNcmTe(myEstV2);
        }
        myEstV2=_estService.CalcTasaEstad061(myEstV2);
        myEstV2=_estService.CalcBaseGcias(myEstV2);
        if(once) 
        {
            myEstV2=await _estService.searchIva(myEstV2);
        }
        myEstV2=_estService.CalcIVA415(myEstV2);
        if(once)
        {
            myEstV2= await _estService.searchIvaAdic(myEstV2);
        }
        myEstV2=_estService.CalcIVA_ad_Gcias(myEstV2);
        myEstV2=_estService.CalcImpGcias424(myEstV2);
        myEstV2=await _estService.CalcIIBB900(myEstV2);
        myEstV2=_estService.CalcPrecioUnitUSS(myEstV2);
        myEstV2=_estService.CalcPagado(myEstV2);
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
        // Hago los calculos forzando (por unica vez) que todos lo datos que vienen de otras tablas sean consultados.
        myEstV2=await calcOnce(myEstV2,true);

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
            myEstV2=await calcOnce(myEstV2,false);
            // Me fijo el valor saliente (este vlaor lo fijo el usuario como DATO)
            tmpOut=(double)adjPropOut.GetValue(myEstV2.EstDetails[0]);


            // cuento la cantidad de iteraciones.
            pass++;
        }

        return myEstV2;
    }
  
}



