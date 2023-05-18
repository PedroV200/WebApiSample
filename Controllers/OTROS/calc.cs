using WebApiSample.Models;
using WebApiSample.Infrastructure;

public class calc
{
    // Con UnitOfWork accedo a todos los repositorios.
    private IUnitOfWork _unitOfWork;

    List<double> cbmTot;
    List<double> pesoTotal;
    List<double> FOB;
    List<double> flete;
    List<double> seguro;
    List<double> CIF;
    List<double> ajusteIncluir;
    List<double> ajusteDeducir;
    List<double> valorAduanaDivisa;
    List<double> diePorct;
    List<double> derechos;
    List<double> tePorct;
    double FOB_TOTAL;
    double SEGURO_TOTAL;
    double CBM_GRAND_TOTAL;
    double CANTIDAD_CONTENEDORES;
    double TARIFA_FLETE_CONTENEDOR;
    public calc(IUnitOfWork unitOfWork)
    {
        _unitOfWork=unitOfWork;
        cbmTot=new List<double>();
        pesoTotal=new List<double>();
        FOB=new List<double>();
        flete=new List<double>();
        seguro=new List<double>();
        CIF=new List<double>();
        ajusteIncluir=new List<double>();
        ajusteDeducir=new List<double>();
        valorAduanaDivisa=new List<double>();
        diePorct=new List<double>(); 
        derechos=new List<double>();
        tePorct=new List<double>();
    }
    // Trae todos los item del detalle cuyo header tiene el codigo "code"
    public async Task<List<EstimateDetail>> getItems(int code)
    {
        var result= await _unitOfWork.EstimateDetails.GetAllByCodeAsync(code);
        return result.Cast<EstimateDetail>().ToList();

    }

        public async Task<EstimateHeader> getHeader(int code)
    {
        var result= await _unitOfWork.EstimateHeaders.GetByIdAsync(code);
        return result;

    }

// Ejecuta el batch de calculos, de izq a derecha conforme el libro de XLS
// Segun "Presupuestador Argentina, libro N - Duchas Escocesas"

    public async Task<List<double>> calcBatch(int code)
    {
        // Cargo la lista del detalle a RAM.
        // Los calculos recorreran esta lista.

        // Trae el Estimate Detail, todo los art que vinculados al header con el codigo "code"
        List<EstimateDetail> estDetails=await getItems(code);
        // Trae el estimate header identificado con el codigo "code"
        EstimateHeader estHeader=await getHeader(code);
        // Segun sea el tipo de contenedor indicado en el Estimate Header
        // busco todos los datos de ese tipo de contenedor en la tala contenedores
        Contenedor estContType= await lookUpCont(estHeader.freighttype);
        // Segun sea el tipo de contenedor y fowarder origen, busco la tarifa
        TarifasFwdCont tarCont= await lookUpTarifaFleteCont(estHeader);
        // Inicio de la propagacion de calculos.
        pesoTotal=calculatePesoTotal(estDetails);
        cbmTot=calculateCBM(estDetails);
        // Total de la lista anterior. Tipicamente K43
        CBM_GRAND_TOTAL = sumar_double(cbmTot);     // CELDA K43
        // Calculo la celda K45 que se referencia en C10
        // Cantidad de contenedores que necesito para cubrir el CBM total.
        CANTIDAD_CONTENEDORES = CBM_GRAND_TOTAL / estContType.volume;   
        FOB=calculateFOB(estDetails);
        FOB_TOTAL=sumar_double(FOB);    // Celda C3
        // Segun la cantidad de contenedores para cubirr el CBM TOT y la tarifa para 
        // para el tipo de contenedor especificado
        TARIFA_FLETE_CONTENEDOR=tarCont.costoflete060*CANTIDAD_CONTENEDORES;  // Usado en formula de CELDA C4
        flete=calculateFlete(FOB);
        SEGURO_TOTAL=FOB_TOTAL*estHeader.seguro;
        seguro=calculateSeguro(FOB);
        CIF=calculateCIF(FOB,flete,seguro);
        // Este ajustes no hacen nada hoy por que aun no se les dio uso en el Presupuestador.
        valorAduanaDivisa=ajustIncCIF(CIF,ajusteIncluir);
        // Este Ajuste no tiene efecto. No se encuentra en uso en el presupuestador
        // Uso a valorAduanaDivisa como un temporal.
        valorAduanaDivisa=ajustDedCIF(valorAduanaDivisa,ajusteDeducir);
        // Traigo el derecho de importacion dado el NCM
        diePorct=await lookUpDie(estDetails);
        derechos=calculateDerechos(valorAduanaDivisa,diePorct);
        // Busca la tasa estadistica segun el NCM. Si no existe, le asigna 3.0%.
        tePorct=await resolveTe(estDetails);
        // Para cumplir con el POST que espera una lista de doubles.
        // Paso los CBMs totales.
        return cbmTot;
    }


    public List<double> calculateCBM(List<EstimateDetail> estDetails)
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
        for(int i=0; i<cifTmp.Count /*&& i<adjIncTmp.Count*/; i++)    
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
        for(int i=0; i<cifTmp.Count /*&& i<adjDedTmp.Count*/; i++)    
        {
            // ADEVERTENCIA: POPULAR CUANDO APAREZCA FORMULA EN COLUMNA Q "Ajuste a Deducir"
            // Por ahora no se hace nada en esa columan. El resultado es el CIF.
            tmpL.Add(cifTmp[i]);
        }  
        return tmpL;
    } 

    public async Task<List<double>> lookUpDie(List<EstimateDetail> estDetails)
    {
        List<double> tmpL=new List<double>();
        foreach(EstimateDetail ed in estDetails)
        {
            NCM myNCM=await _unitOfWork.NCMs.GetByIdStrAsync(ed.ncm); 
            // Lo agrega a la lista
            tmpL.Add(myNCM.die);   
        }       
        return tmpL;
    }

    public async Task<Contenedor> lookUpCont(string type)
    {
        Contenedor myCont=await _unitOfWork.Contenedores.GetByTipoContAsync(type);  
    
        return myCont;
    }

    public async Task<TarifasFwdCont> lookUpTarifaFleteCont(EstimateHeader estHeader)
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
    }
}