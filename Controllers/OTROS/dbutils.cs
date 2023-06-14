using WebApiSample.Models;
using WebApiSample.Infrastructure;

public class dbutils
{
    public IUnitOfWork _unitOfWork;
    public dbutils(IUnitOfWork unitOfowrk)
    {
        _unitOfWork=unitOfowrk;
    }

    public async Task<EstimateDB> getEstimateLastVers(int estNumber)
    {
        EstimateDB myEst=new EstimateDB(); 
        // La consulta de headers por un numero de presupuesto puede dar como resultado mas
        // de un header (multiples versiones del mismo presupuesto). 
        List<EstimateHeaderDB>misDetalles=new List<EstimateHeaderDB>();
        // La query se hace ORDENADA POR VERSION de MAYOR A MENOR. Es una LISTA de estHeaders
        var result=await _unitOfWork.EstimateHeadersDB.GetByEstNumberLastVersAsync(estNumber);
        misDetalles=result.ToList();
        // El elemento 0 corresponde al header buscado en con la version MAYOR.
        myEst.estHeaderDB=misDetalles[0];
        // Con el PK del header anterior me voy a buscar los Details que lo referencia en su FK
        var result1=await _unitOfWork.EstimateDetailsDB.GetAllByIdEstHeadersync(myEst.estHeaderDB.Id);
        // De nuevo, la consulta x detalles, da una LISTA como resultado.
        myEst.estDetailsDB=result1.ToList();
        // Devuelvo el presupuesto.
        return myEst;
    }

    public async Task<EstimateDB> getEstimateFirstVers(int estNumber)
    {
        EstimateDB myEst=new EstimateDB(); 
        // La consulta de headers por un numero de presupuesto puede dar como resultado mas
        // de un header (multiples versiones del mismo presupuesto). 
        List<EstimateHeaderDB>misDetalles=new List<EstimateHeaderDB>();
        // La query se hace ORDENADA POR VERSION de MAYOR A MENOR. Es una LISTA de estHeaders
        var result=await _unitOfWork.EstimateHeadersDB.GetByEstNumberLastVersAsync(estNumber);
        misDetalles=result.ToList();
        // El elemento n-1 corresponde al header buscado en con la version MENOR.
        myEst.estHeaderDB=misDetalles[misDetalles.Count-1];
        // Con el PK del header anterior me voy a buscar los Details que lo referencia en su FK
        var result1=await _unitOfWork.EstimateDetailsDB.GetAllByIdEstHeadersync(myEst.estHeaderDB.Id);
        // De nuevo, la consulta x detalles, da una LISTA como resultado.
        myEst.estDetailsDB=result1.ToList();
        // Devuelvo el presupuesto.
        return myEst;
    }

    public async Task<int> deleteEstimateByNumByVers(int estNumber, int estVers)
    {
        EstimateHeaderDB myEst=new EstimateHeaderDB();
        myEst=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(estNumber,estVers);
        // Primero liquido los estDetails del ESTIMATE. No puedo borrar primero el header por que las FK quedan sueltas.
        // Primero borro los estDetail con FK coinciente.
        var result= await _unitOfWork.EstimateDetailsDB.DeleteByIdEstHeaderAsync(myEst.Id);
        // Ahora si borro el header
        var result1=await _unitOfWork.EstimateHeadersDB.DeleteAsync(myEst.Id);
        // devuelvo solo la cantidad de estimateDetail borrados, por que header ya se que borre uno solo.
        return result;
    }

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
        myEstV2.OemSupplier=estimateDB.estHeaderDB.oemprove1;
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
        estimateDB.estHeaderDB.oemprove1=myEstV2.OemSupplier;
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


}


 