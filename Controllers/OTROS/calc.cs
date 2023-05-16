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
    double FOB_TOTAL;
    double FLETE_TOTAL;             // Esto debe tomarse de otra base segun tipo de contenedor
    double SEGURO_TOTAL;
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
    }
    // Trae todos los item del detalle cuyo header tiene el codigo "code"
    public async Task<List<EstimateDetail>> getItems(int code)
    {
        var result= await _unitOfWork.EstimateDetails.GetAllByCodeAsync(code);
        return result.Cast<EstimateDetail>().ToList();

    }

// Ejecuta el batch de calculos, de izq a derecha conforme el libro de XLS
// Segun "Presupuestador Argentina, libro N - Duchas Escocesas"

    public async Task<List<double>> calcBatch(int code)
    {
        // Cargo la lista del detalle a RAM.
        // Los calculos recorreran esta lista.
        List<EstimateDetail> estDetails=await getItems(code);
        // Inicio de la propagacion de calculos.
        pesoTotal=calculatePesoTotal(estDetails);
        cbmTot=calculateCBM(estDetails);
        FOB=calculateFOB(estDetails);
        FOB_TOTAL=sumar_double(FOB);    // Celda C3
        // ADVERTENCIA: FALTA LINKEAR "FLETE_TOTAL" CELDA C4 !!!!!
        flete=calculateFlete(FOB);
        // ADVERTENCIA: FALTA LINKEAR "SEGURO_TOTAL" CELDA C5 !!!!
        seguro=calculateSeguro(FOB);
        CIF=calculateCIF(FOB,flete,seguro);
        valorAduanaDivisa=ajustIncCIF(CIF,ajusteIncluir);
        // Uso a valorAduanaDivisa como un temporal.
        valorAduanaDivisa=ajustDedCIF(valorAduanaDivisa,ajusteDeducir);
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
            tmp=(d_FOB/FOB_TOTAL)*FLETE_TOTAL; 
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
        for(int i=0; i<cifTmp.Count && i<adjIncTmp.Count; i++)    
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
        for(int i=0; i<cifTmp.Count && i<adjDedTmp.Count; i++)    
        {
            // ADEVERTENCIA: POPULAR CUANDO APAREZCA FORMULA EN COLUMNA Q "Ajuste a Deducir"
            // Por ahora no se hace nada en esa columan. El resultado es el CIF.
            tmpL.Add(cifTmp[i]);
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