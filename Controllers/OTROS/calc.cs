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
        List<EstimateHeaderDB> misDetalles=new List<EstimateHeaderDB>();
        EstimateDB myEstDB=new EstimateDB(); 
        // Me traigo la ultima version del estNumber pasado como parametro.
        var result=await _unitOfWork.EstimateHeadersDB.GetByEstNumberLastVersAsync(estNumber);
        misDetalles=result.ToList();
        myEstDB.estHeaderDB=misDetalles[0];
        // Del header obtenido en la consulta anterior, me interesa la PK (Id) por que es FK en 
        // la tabla estimate details. Busco todos los productos que tengan con FK coincidente con el ID (PK en estHeader)
        var result1=await _unitOfWork.EstimateDetailsDB.GetAllByIdEstHeadersync(myEstDB.estHeaderDB.Id);
        // Lo paso a una lista.
        myEstDB.estDetailsDB=result1.ToList();


        // El objeto Estimate que se definio. 
        EstimateV2 myEstV2=new EstimateV2();

        // Expando el EstimateDB a un EstimateV2
        myEstV2=transferDataFromDBType(myEstDB);

        // Hago algunas cuentas.
        myEstV2=_estService.CalcPesoTotal(myEstV2);
        myEstV2=_estService.CalcFobTotal(myEstV2);
        myEstV2=_estService.CalcFactorProdTotal(myEstV2);
        // Fin de las cuentas.

// Tengo que devolver los pesos totales en el POST calculado como parte del ejemplo
        List<double> pesoTot=new List<double>();
// Para cada porducto del detalel estraigo el peso Total:
        foreach(EstimateDetail ed in myEstV2.EstDetails)
        {
            pesoTot.Add(ed.PesoTot);
        }
// Devuelvo la lista de lo que saque.
        return pesoTot;
    }

    // Existen 2 clases de Etimate. EstimateDB que es %100 compatible con las tablas
    // estimateDetail y EstimateHeader en la BD. Y luego esta EstimateV2 que se definio
    // segun lo charlado.
    // Esta funcion pasa un EstimateDB a un EstimateV2.
    public EstimateV2 transferDataFromDBType(EstimateDB estimateDB)
    {
        EstimateV2 myEstV2=new EstimateV2();

        // EstimateV2 no cuenta con un header. Los datos del header se encuentran directamente
        // como propiedades  .....
        myEstV2.Id=estimateDB.estHeaderDB.Id;
        myEstV2.Description=estimateDB.estHeaderDB.Description;
        myEstV2.EstNumber=estimateDB.estHeaderDB.EstNumber;
        myEstV2.EstVers=estimateDB.estHeaderDB.EstNumber;
        myEstV2.Owner=estimateDB.estHeaderDB.Own;
        myEstV2.ArticleFamily=estimateDB.estHeaderDB.ArticleFamily;
        myEstV2.OemSupplier=estimateDB.estHeaderDB.OemSupplier;
        myEstV2.IvaExcento=estimateDB.estHeaderDB.IvaExcento;
        myEstV2.DollarBillete=estimateDB.estHeaderDB.DollarBillete;
        myEstV2.FreightType=estimateDB.estHeaderDB.FreightType;
        myEstV2.FreightFwd=estimateDB.estHeaderDB.FreightFwd;
        myEstV2.TimeStamp=estimateDB.estHeaderDB.hTimeStamp;
        myEstV2.FobGrandTotal=estimateDB.estHeaderDB.FobGrandTotal;
        myEstV2.FleteTotal=estimateDB.estHeaderDB.FleteTotal;
        myEstV2.Seguro=estimateDB.estHeaderDB.Seguro;
        myEstV2.Seguroporct=estimateDB.estHeaderDB.SeguroPorct;
        myEstV2.CantidadContenedores=estimateDB.estHeaderDB.CantidadContenedores;
        myEstV2.Pagado=estimateDB.estHeaderDB.Pagado;

        // En el caso de los Estimate detail, la DB tiene solo lo datos que se cuardaran
        // Este tipo se llama EstimateDetailDB. Mientras que el EstimateV2 definido usa el tipo EstimateDetail
        // que contiene no solo los datos que contiene "EstimateDetailDB" sino ademas provicion para los datos
        // calculados. Estos no se guardan en la base. De ahi la existencia de 2 clases "EstimateDetail"
        foreach(EstimateDetailDB edb in estimateDB.estDetailsDB)
        {
            EstimateDetail tmp=new EstimateDetail();
            tmp.id=edb.Id;
            tmp.modelo=edb.Modelo;
            tmp.ncm=edb.Ncm;
            tmp.pesounitxcaja=edb.PesoUnitxCaja;
            tmp.cbmxcaja=edb.CbmxCaja;
            tmp.pcsxcaja=edb.PcsxCaja;
            tmp.fobunit=edb.FobUnit;
            tmp.cantpcs=edb.CantPcs;
            tmp.idestheader=edb.IdEstHeader;
            myEstV2.EstDetails.Add(tmp);
        }
        return myEstV2;
    }
// La inversa de la anterior. Pasa un EstimateV2 a un EstimateDB.
    public EstimateDB transferDataToDBType(EstimateV2 myEstV2)
    {
         EstimateDB estimateDB=new EstimateDB();


        estimateDB.estHeaderDB.Id=myEstV2.Id;
        estimateDB.estHeaderDB.Description=myEstV2.Description;
        estimateDB.estHeaderDB.EstNumber=myEstV2.EstNumber;
        estimateDB.estHeaderDB.EstNumber=myEstV2.EstVers;
        estimateDB.estHeaderDB.Own=myEstV2.Owner;
        estimateDB.estHeaderDB.ArticleFamily=myEstV2.ArticleFamily;
        estimateDB.estHeaderDB.OemSupplier=myEstV2.OemSupplier;
        estimateDB.estHeaderDB.IvaExcento=myEstV2.IvaExcento;
        estimateDB.estHeaderDB.DollarBillete=myEstV2.DollarBillete;
        estimateDB.estHeaderDB.FreightType=myEstV2.FreightType;
        estimateDB.estHeaderDB.FreightFwd=myEstV2.FreightFwd;
        estimateDB.estHeaderDB.hTimeStamp=myEstV2.TimeStamp;
        estimateDB.estHeaderDB.FobGrandTotal=myEstV2.FobGrandTotal;
        estimateDB.estHeaderDB.FleteTotal=myEstV2.FleteTotal;
        estimateDB.estHeaderDB.Seguro=myEstV2.Seguro;
        estimateDB.estHeaderDB.SeguroPorct=myEstV2.Seguroporct;
        estimateDB.estHeaderDB.CantidadContenedores=myEstV2.CantidadContenedores;
        estimateDB.estHeaderDB.Pagado=myEstV2.Pagado;

// Aqui se descartan los calculos. Solo se transfieren los valores necesarios para los mismos
        foreach(EstimateDetail edb in myEstV2.EstDetails)
        {
            EstimateDetailDB tmp=new EstimateDetailDB();
            tmp.Id=edb.id;
            tmp.Modelo=edb.modelo;
            tmp.Ncm=edb.ncm;
            tmp.PesoUnitxCaja=edb.pesounitxcaja;
            tmp.CbmxCaja=edb.cbmxcaja;
            tmp.PcsxCaja=edb.pcsxcaja;
            tmp.FobUnit=edb.fobunit;
            tmp.CantPcs=edb.cantpcs;
            tmp.IdEstHeader=edb.idestheader;
            estimateDB.estDetailsDB.Add(tmp);
        }
        return estimateDB;       
    }

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