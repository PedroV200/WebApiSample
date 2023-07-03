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
        if(result==null)
        {
            
            return null;
        }
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

    public async Task<EstimateDB> GetEstimateByNumByVers(int estNumber, int estVers)
    {
        EstimateDB myEst=new EstimateDB();
        myEst.estHeaderDB=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(estNumber,estVers);
        if(myEst.estHeaderDB==null)
        {
            return null;
        }
        var result=await _unitOfWork.EstimateDetailsDB.GetAllByIdEstHeadersync(myEst.estHeaderDB.Id);
        myEst.estDetailsDB=result.ToList();
        return myEst;
    }

    public EstimateV2 transferDataFromDBType(EstimateDB estimateDB)
    {
        EstimateV2 myEstV2=new EstimateV2();

        // EstimateV2 no cuenta con un header. Los datos del header se encuentran directamente
        // como propiedades  .....
        myEstV2.Id=estimateDB.estHeaderDB.Id;
        myEstV2.Description=estimateDB.estHeaderDB.Description;
        myEstV2.EstNumber=estimateDB.estHeaderDB.EstNumber;
        myEstV2.EstVers=estimateDB.estHeaderDB.EstVers;
        myEstV2.Owner=estimateDB.estHeaderDB.Own;
        myEstV2.ArticleFamily=estimateDB.estHeaderDB.ArticleFamily;
        myEstV2.IvaExcento=estimateDB.estHeaderDB.IvaExcento;
        
        myEstV2.FreightType=estimateDB.estHeaderDB.FreightType;
        myEstV2.FreightFwd=estimateDB.estHeaderDB.FreightFwd;
        myEstV2.TimeStamp=estimateDB.estHeaderDB.hTimeStamp;
        myEstV2.FobGrandTotal=estimateDB.estHeaderDB.FobGrandTotal;
        myEstV2.FleteTotal=estimateDB.estHeaderDB.FleteTotal;
        myEstV2.Seguro=estimateDB.estHeaderDB.Seguro;
        //myEstV2.ArancelSim=estimateDB.estHeaderDB.ArancelSim;
        //myEstV2.SeguroPorct=estimateDB.estHeaderDB.SeguroPorct;
        myEstV2.CantidadContenedores=estimateDB.estHeaderDB.CantidadContenedores;
        myEstV2.Pagado=estimateDB.estHeaderDB.Pagado;
        myEstV2.CbmGrandTot=estimateDB.estHeaderDB.CbmTot;
        myEstV2.CifTot=estimateDB.estHeaderDB.CifTot;
        myEstV2.IibbTot=estimateDB.estHeaderDB.IibbTot;
        myEstV2.PolizaProv=estimateDB.estHeaderDB.PolizaProv;
        myEstV2.ExtraGastosLocProyectado=estimateDB.estHeaderDB.ExtraGastosLocProyectado;  

        // Cargo las Constantes ....
        myEstV2.constantes.CNST_ESTAD061_ThrhldMAX=estimateDB.estHeaderDB.c_est061_thrhldmax;
        myEstV2.constantes.CNST_ESTAD061_ThrhldMIN=estimateDB.estHeaderDB.c_est061_thrhldmin;
        myEstV2.constantes.CNST_GASTOS_BANCARIOS_Mult=estimateDB.estHeaderDB.c_gbanc_mult;
        myEstV2.constantes.CNST_GCIAS_424_Mult=estimateDB.estHeaderDB.c_gcias424_mult;
        myEstV2.constantes.CNST_GASTOS_CUSTODIA_Thrshld=estimateDB.estHeaderDB.c_gcust_thrshld;
        myEstV2.constantes.CNST_GASTOS_DESPA_Cif_Min=estimateDB.estHeaderDB.c_gdespa_cif_min;
        myEstV2.constantes.CNST_GASTOS_DESPA_Cif_Mult=estimateDB.estHeaderDB.c_gdespa_cif_mult;
        myEstV2.constantes.CNST_GASTOS_DESPA_Cif_Thrhld=estimateDB.estHeaderDB.c_gdespa_cif_thrhld;
        myEstV2.constantes.CNST_GASTOS_GESTDIGDOC_Mult=estimateDB.estHeaderDB.c_ggesdoc_mult;
        myEstV2.constantes.CONST_NCM_DIE_Min=estimateDB.estHeaderDB.c_ncmdie_min;
        myEstV2.constantes.CNST_SEGURO_PORCT=estimateDB.estHeaderDB.c_seguroporct;
        myEstV2.constantes.CNST_ARANCEL_SIM=estimateDB.estHeaderDB.c_arancelsim;

        myEstV2.DolarBillete=estimateDB.estHeaderDB.DolarBillete;


        myEstV2.p_gloc_banco=estimateDB.estHeaderDB.p_gloc_banco;
        myEstV2.p_gloc_cust=estimateDB.estHeaderDB.p_gloc_cust;
        myEstV2.p_gloc_despa=estimateDB.estHeaderDB.p_gloc_despa;
        myEstV2.p_gloc_gestdigdoc=estimateDB.estHeaderDB.p_gloc_gestdigdoc;
        myEstV2.p_gloc_fwder=estimateDB.estHeaderDB.p_gloc_fwder;
        myEstV2.p_gloc_term=estimateDB.estHeaderDB.p_gloc_term;
        myEstV2.p_gloc_tte=estimateDB.estHeaderDB.p_gloc_tte;

        myEstV2.oemprove1=estimateDB.estHeaderDB.oemprove1;
        myEstV2.oemprove2=estimateDB.estHeaderDB.oemprove2;
        myEstV2.oemprove3=estimateDB.estHeaderDB.oemprove3;
        myEstV2.oemprove4=estimateDB.estHeaderDB.oemprove4;
        myEstV2.oemprove5=estimateDB.estHeaderDB.oemprove5;
        myEstV2.oemprove6=estimateDB.estHeaderDB.oemprove6;
        myEstV2.oemprove7=estimateDB.estHeaderDB.oemprove7;

        myEstV2.id_PolizaProv=estimateDB.estHeaderDB.id_PolizaProv;
        myEstV2.id_p_gloc_banco=estimateDB.estHeaderDB.id_p_gloc_banco;
        myEstV2.id_p_gloc_cust=estimateDB.estHeaderDB.id_p_gloc_cust;
        myEstV2.id_p_gloc_despa=estimateDB.estHeaderDB.id_p_gloc_despa;
        myEstV2.id_p_gloc_gestdigdoc=estimateDB.estHeaderDB.id_p_gloc_gestdigdoc;
        myEstV2.id_p_gloc_fwder=estimateDB.estHeaderDB.id_p_gloc_fwder;
        myEstV2.id_p_gloc_term=estimateDB.estHeaderDB.id_p_gloc_term;
        myEstV2.id_p_gloc_tte=estimateDB.estHeaderDB.id_p_gloc_tte;

        myEstV2.id_oemprove1=estimateDB.estHeaderDB.id_oemprove1;
        myEstV2.id_oemprove2=estimateDB.estHeaderDB.id_oemprove2;
        myEstV2.id_oemprove3=estimateDB.estHeaderDB.id_oemprove3;
        myEstV2.id_oemprove4=estimateDB.estHeaderDB.id_oemprove4;
        myEstV2.id_oemprove5=estimateDB.estHeaderDB.id_oemprove5;
        myEstV2.id_oemprove6=estimateDB.estHeaderDB.id_oemprove6;
        myEstV2.id_oemprove7=estimateDB.estHeaderDB.id_oemprove7;


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
            tmp.ncm_die=edb.ncm_die;
            tmp.ncm_te=edb.ncm_te;
            tmp.ncm_iva=edb.ncm_iva;
            tmp.ncm_ivaad=edb.ncm_ivaad;
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
        estimateDB.estHeaderDB.EstVers=myEstV2.EstVers;
        estimateDB.estHeaderDB.Own=myEstV2.Owner;
        estimateDB.estHeaderDB.ArticleFamily=myEstV2.ArticleFamily;
        estimateDB.estHeaderDB.IvaExcento=myEstV2.IvaExcento;
        
        estimateDB.estHeaderDB.FreightType=myEstV2.FreightType;
        estimateDB.estHeaderDB.FreightFwd=myEstV2.FreightFwd;
        estimateDB.estHeaderDB.hTimeStamp=myEstV2.TimeStamp;
        estimateDB.estHeaderDB.FobGrandTotal=myEstV2.FobGrandTotal;
        estimateDB.estHeaderDB.FleteTotal=myEstV2.FleteTotal;
        estimateDB.estHeaderDB.Seguro=myEstV2.Seguro;
        //estimateDB.estHeaderDB.ArancelSim=myEstV2.ArancelSim;
        //estimateDB.estHeaderDB.SeguroPorct=myEstV2.SeguroPorct;
        estimateDB.estHeaderDB.CantidadContenedores=myEstV2.CantidadContenedores;
        estimateDB.estHeaderDB.Pagado=myEstV2.Pagado;

        estimateDB.estHeaderDB.CbmTot=myEstV2.CbmGrandTot;
        estimateDB.estHeaderDB.CifTot=myEstV2.CifTot;
        estimateDB.estHeaderDB.IibbTot=myEstV2.IibbTot;

        estimateDB.estHeaderDB.ExtraGastosLocProyectado=myEstV2.ExtraGastosLocProyectado;  

        // Cargo las Constantes ....
        estimateDB.estHeaderDB.c_est061_thrhldmax=myEstV2.constantes.CNST_ESTAD061_ThrhldMAX;
        estimateDB.estHeaderDB.c_est061_thrhldmin=myEstV2.constantes.CNST_ESTAD061_ThrhldMIN;
        estimateDB.estHeaderDB.c_gbanc_mult=myEstV2.constantes.CNST_GASTOS_BANCARIOS_Mult;
        estimateDB.estHeaderDB.c_gcias424_mult=myEstV2.constantes.CNST_GCIAS_424_Mult;
        estimateDB.estHeaderDB.c_gcust_thrshld=myEstV2.constantes.CNST_GASTOS_CUSTODIA_Thrshld;
        estimateDB.estHeaderDB.c_gdespa_cif_min=myEstV2.constantes.CNST_GASTOS_DESPA_Cif_Min;
        estimateDB.estHeaderDB.c_gdespa_cif_mult=myEstV2.constantes.CNST_GASTOS_DESPA_Cif_Mult;
        estimateDB.estHeaderDB.c_gdespa_cif_thrhld=myEstV2.constantes.CNST_GASTOS_DESPA_Cif_Thrhld;
        estimateDB.estHeaderDB.c_ggesdoc_mult=myEstV2.constantes.CNST_GASTOS_GESTDIGDOC_Mult;
        estimateDB.estHeaderDB.c_ncmdie_min=myEstV2.constantes.CONST_NCM_DIE_Min;
        estimateDB.estHeaderDB.c_seguroporct=myEstV2.constantes.CNST_SEGURO_PORCT;
        estimateDB.estHeaderDB.c_arancelsim=myEstV2.constantes.CNST_ARANCEL_SIM;
        
        estimateDB.estHeaderDB.DolarBillete=myEstV2.DolarBillete;
        
        estimateDB.estHeaderDB.PolizaProv=myEstV2.PolizaProv;
        estimateDB.estHeaderDB.p_gloc_banco=myEstV2.p_gloc_banco;
        estimateDB.estHeaderDB.p_gloc_cust=myEstV2.p_gloc_cust;
        estimateDB.estHeaderDB.p_gloc_despa=myEstV2.p_gloc_despa;
        estimateDB.estHeaderDB.p_gloc_gestdigdoc=myEstV2.p_gloc_gestdigdoc;
        estimateDB.estHeaderDB.p_gloc_fwder=myEstV2.p_gloc_fwder;
        estimateDB.estHeaderDB.p_gloc_term=myEstV2.p_gloc_term;
        estimateDB.estHeaderDB.p_gloc_tte=myEstV2.p_gloc_tte;

        estimateDB.estHeaderDB.oemprove1=myEstV2.oemprove1;
        estimateDB.estHeaderDB.oemprove2=myEstV2.oemprove2;
        estimateDB.estHeaderDB.oemprove3=myEstV2.oemprove3;
        estimateDB.estHeaderDB.oemprove4=myEstV2.oemprove4;
        estimateDB.estHeaderDB.oemprove5=myEstV2.oemprove5;
        estimateDB.estHeaderDB.oemprove6=myEstV2.oemprove6;
        estimateDB.estHeaderDB.oemprove7=myEstV2.oemprove7;

        estimateDB.estHeaderDB.id_PolizaProv=myEstV2.id_PolizaProv;
        estimateDB.estHeaderDB.id_p_gloc_banco=myEstV2.id_p_gloc_banco;
        estimateDB.estHeaderDB.id_p_gloc_cust=myEstV2.id_p_gloc_cust;
        estimateDB.estHeaderDB.id_p_gloc_despa=myEstV2.id_p_gloc_despa;
        estimateDB.estHeaderDB.id_p_gloc_gestdigdoc=myEstV2.id_p_gloc_gestdigdoc;
        estimateDB.estHeaderDB.id_p_gloc_fwder=myEstV2.id_p_gloc_fwder;
        estimateDB.estHeaderDB.id_p_gloc_term=myEstV2.id_p_gloc_term;
        estimateDB.estHeaderDB.id_p_gloc_tte=myEstV2.id_p_gloc_tte;

        estimateDB.estHeaderDB.id_oemprove1=myEstV2.id_oemprove1;
        estimateDB.estHeaderDB.id_oemprove2=myEstV2.id_oemprove2;
        estimateDB.estHeaderDB.id_oemprove3=myEstV2.id_oemprove3;
        estimateDB.estHeaderDB.id_oemprove4=myEstV2.id_oemprove4;
        estimateDB.estHeaderDB.id_oemprove5=myEstV2.id_oemprove5;
        estimateDB.estHeaderDB.id_oemprove6=myEstV2.id_oemprove6;
        estimateDB.estHeaderDB.id_oemprove7=myEstV2.id_oemprove7;

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
            edb.ncm_die=tmp.ncm_die;
            edb.ncm_te=tmp.ncm_te;
            edb.ncm_iva=tmp.ncm_iva;
            edb.ncm_ivaad=tmp.ncm_ivaad;
            estimateDB.estDetailsDB.Add(tmp);
        }
        return estimateDB;       
    }


}


 