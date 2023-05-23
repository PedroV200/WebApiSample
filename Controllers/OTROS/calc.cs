using WebApiSample.Models;
using WebApiSample.Infrastructure;

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
    public async Task<List<double>> calcBatch(int estNumber)
    {
        EstimateDB myEstDB=new EstimateDB(); 
        dbutils dbhelper=new dbutils(_unitOfWork);
        myEstDB=await dbhelper.getEstimateLastVers(estNumber);


        // El objeto Estimate que se definio. 
        EstimateV2 myEstV2=new EstimateV2();

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
        myEstV2.FleteTotal=await lookUpTarifaFleteCont(myEstV2);
        // COL M. Calcula el flete ponderado a cada articulo del detalle.
        myEstV2=_estService.CalcFleteTotal(myEstV2);
        // COL N. Calcula el seguro ponderado a cada articulo del detalle 
        myEstV2=_estService.CalcSeguro(myEstV2);
        // CELDA AH. Calculo el factor de producto
        myEstV2=_estService.CalcFactorProdTotal(myEstV2);
        // COL O. Calcula el CIF que solo depende de los datos ya calculados previamente (COL L, N y M)
        myEstV2=_estService.CalcCif(myEstV2);
        // COL R (COL O y COL Q no estan en uso)
        myEstV2=_estService.CalcAjusteIncDec(myEstV2);
        // COL S (die segun NCM)
        myEstV2=await _estService.searchNcmDie(myEstV2);

        // Fin de las cuentas.

// Tengo que devolver los pesos totales en el POST calculado como parte del ejemplo
        List<double> pesoTot=new List<double>();
// Para cada porducto del detalle estraigo el peso Total:
        foreach(EstimateDetail ed in myEstV2.EstDetails)
        {
            pesoTot.Add(ed.PesoTot);
        }
// Devuelvo la lista de lo que saque.
        return pesoTot;
    }

 


    public async Task<double> lookUpTarifaFleteCont(EstimateV2 est)
    {
        TarifasFwdCont myTarCont=await _unitOfWork.TarifasFwdContenedores.GetByFwdContTypeAsync(est.FreightFwd,est.FreightType); 

        if(myTarCont!=null)
        { 
            return myTarCont.costoflete060;
        }
        return -1;
    }

    // Existen 2 clases de Etimate. EstimateDB que es %100 compatible con las tablas
    // estimateDetail y EstimateHeader en la BD. Y luego esta EstimateV2 que se definio
    // segun lo charlado.
    // Esta funcion pasa un EstimateDB a un EstimateV2.
    

 /*   public List<double> calculateCBM(List<EstimateDetail> estDetails)
    {
        double tmp=0;
        List<double> tmpL=new List<double>();
        foreach(EstimateDetail ed in estDetails)
        {
            // Guarda con entradas rotas que tiene el pcsxcaja en 0.
            if(ed.pcsxcaja!=0)
            {
                tmp=(ed.cantpcs*ed.cbmxcaja)/ed.pcsxcaja; 
            }
            // Lo agrega a la lista
            tmpL.Add(tmp);
        }
       
        return tmpL;
    }

    public List<double> calculatePesoTotal(List<EstimateDetail> estDetails)
    {
        double tmp=0;
        List<double> tmpL=new List<double>();
        foreach(EstimateDetail ed in estDetails)
        {
            // Guarda con entradas rotas que tiene el pcsxcaja en 0.
            if(ed.pcsxcaja!=0)
            {
                tmp=(ed.pesounitxcaja/ed.pcsxcaja)*ed.cantpcs; 
            }
            // Lo agrega a la lista
            tmpL.Add(tmp);   
        }
       
        return tmpL;
    }
// Usa mas de un parametro en el calculo. Paso la lista del detalle que esta en RAM
    public List<double> calculateFOB(List<EstimateDetail> estDetails)
    {
        double tmp=0;
        List<double> tmpL=new List<double>();
        foreach(EstimateDetail ed in estDetails)
        {
            tmp=ed.fobunit*ed.cantpcs; 
            // Lo agrega a la lista
            tmpL.Add(tmp);   
        }       
        return tmpL;
    }
// Solo barre sobre una columna (FOB), con lo que recibe es una lista de Doubles.
    public List<double> calculateFlete(List<double> misValores)
    {
        int i=0;
        double tmp=0;
        List<double> tmpL=new List<double>();
        foreach(Double d_FOB in misValores)
        {
            i++;
            // Formula
            tmp=(d_FOB/FOB_TOTAL)*TARIFA_FLETE_CONTENEDOR; 
            // Lo agrega a la lista
            tmpL.Add(tmp);   
        }       
        return tmpL;
    } 

    // Solo barre sobre una columna (FOB), con lo que recibe es una lista de Doubles.
    public List<double> calculateSeguro(List<double> fobTmp)
    {
        double tmp=0;
        List<double> tmpL=new List<double>();
        foreach(Double d_FOB in fobTmp)
        {
            // Formula
            tmp=(d_FOB/FOB_TOTAL)*SEGURO_TOTAL; 
            // Lo agrega a la lista
            tmpL.Add(tmp);   
        }       
        return tmpL;
    }   

    public List<double>calculateCIF(List<double> fobTmp,List<double> fleteTmp,List<double> seguroTmp)
    {
        double tmp=0;
        List<double> tmpL=new List<double>();
        for(int i=0; i<fobTmp.Count && i<fleteTmp.Count && i<seguroTmp.Count; i++)    
        {
            tmpL.Add(fobTmp[i]+fleteTmp[i]+seguroTmp[i]);
        }  
        return tmpL;
    }   

    public List<double>ajustIncCIF(List<double> cifTmp,List<double> adjIncTmp)
    {
        double tmp=0;
        List<double> tmpL=new List<double>();
        for(int i=0; i<cifTmp.Count /*&& i<adjIncTmp.Count; i++)    
        {
            // ADEVERTENCIA: POPULAR CUANDO APAREZCA FORMULA EN COLUMNA P "Ajuste a Incluir"
            // Por ahora no se hace nada en esa columan. El resultado es el CIF.
            tmpL.Add(cifTmp[i]);
        }  
        return tmpL;
    } 

    public List<double>ajustDedCIF(List<double> cifTmp,List<double> adjDedTmp)
    {
        double tmp=0;
        List<double> tmpL=new List<double>();
        for(int i=0; i<cifTmp.Count /*&& i<adjDedTmp.Count; i++)    
        {
            // ADEVERTENCIA: POPULAR CUANDO APAREZCA FORMULA EN COLUMNA Q "Ajuste a Deducir"
            // Por ahora no se hace nada en esa columan. El resultado es el CIF.
            tmpL.Add(cifTmp[i]);
        }  
        return tmpL;
    } 

    

    public async Task<Contenedor> lookUpCont(string type)
    {
        Contenedor myCont=await _unitOfWork.Contenedores.GetByTipoContAsync(type);  
    
        return myCont;
    }

    public async Task<TarifasFwdCont> lookUpTarifaFleteCont(EstimateHeaderDB estHeader)
    {
        TarifasFwdCont myTarCont=await _unitOfWork.TarifasFwdContenedores.GetByFwdContTypeAsync(estHeader.freightfwd,estHeader.freighttype);  
    
        return myTarCont;
    }
    
    // Busca el TE correspondiente en la tabla dado el NCM para cada articulo
    // En el EXCEL, si no es posible encontrar un TE para un NCM dado, el mismo
    // sera del 3%.
    public async Task<List<double>> resolveTe(List<EstimateDetail> estDetails)
    {
        List<double> tmpL=new List<double>();
        foreach(EstimateDetail ed in estDetails)
        {
            NCM myNCM=await _unitOfWork.NCMs.GetByIdStrAsync(ed.ncm); 
            // VER COLUMNA U (U15 en adelante).
            // Existe el te ?. No puedo tener un te "EN BLANCO" como el XLS. Lo ideal que x defecto tengan un valor negativo
            // como para indicar que esta "en blanco".
            if(myNCM!=null && myNCM.te!>=0)
            { // Si.
                tmpL.Add(myNCM.te);   
            }
            else
            { // No, entonces vale 3%.
                tmpL.Add(3.00000);
            }
        }       
        return tmpL;
    }
    public List<double> calculateDerechos(List<double> tmpValAduanaDivisa, List<double> tmpDiePorct)
    {
        List<double> tmpL=new List<double>();
        for(int i=0;i<tmpValAduanaDivisa.Count && i<tmpDiePorct.Count; i++)
        {
            tmpL.Add(tmpDiePorct[i]*tmpValAduanaDivisa[i]);       
        }
        return tmpL;
    }

// Suma una lista de valores double. Devielve el resultado
    public double sumar_double(List<double> misValores)
    {
        double tmp=0;
        foreach(double d in misValores)
        {
            tmp+=d;
        }
        return tmp;
    }*/
}