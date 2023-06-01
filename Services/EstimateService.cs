namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class EstimateService: IEstimateService
{
    IUnitOfWork _unitOfWork;
    public EstimateService(IEstimateDetailService estDetailServices,IUnitOfWork unitOfWork)
    {
        estDetServices=estDetailServices;
       _unitOfWork=unitOfWork;

    }
    public IEstimateDetailService estDetServices {get;}

    public EstimateV2 CalcPesoTotal(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.PesoTot=estDetServices.CalcPesoTotal(ed);       
        }
        return est;
    }



    public EstimateV2 CalcFobTotal(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.Fob=estDetServices.CalcFob(ed);       
        }
        return est;
    }

    public EstimateV2 CalcCbmTotal(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.CbmTot=estDetServices.CalcCbmTotal(ed);       
        }
        return est;
    }

    public EstimateV2 CalcFleteTotal(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.Flete=estDetServices.CalcFlete(ed,est.FleteTotal,est.FobGrandTotal);       
        }
        return est;
    }

    public EstimateV2 CalcSeguro(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {                                               // CELDA C5=0.1*C3
            ed.Seguro=estDetServices.CalcSeguro(ed,(est.Seguro*est.Seguroporct),est.FobGrandTotal);       
        }
        return est;
    }

    public EstimateV2 CalcCif(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {                                               
            ed.Cif=estDetServices.CalcCif(ed);
        }
        return est;
    }

    // En las columnas P y Q no se hace nada actualmente. 
    // El resultado de los ajsutes es la columna R designada como ValorAduanaEndivisa
    public EstimateV2 CalcAjusteIncDec(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {                                               
            ed.valAduanaDivisa=estDetServices.CalcValorEnAduanaDivisa(ed);
        }
        return est;
    }

    public async Task<EstimateV2> searchNcmDie(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {                                               
            ed.Die=(await estDetServices.lookUpDie(ed))/100.0;
        }
        return est;
    }

    public EstimateV2 CalcDerechos(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {                                               
            ed.Derechos=estDetServices.CalcDerechos(ed);
        }
        return est;
    }  

    public async Task<EstimateV2> resolveNcmTe(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.Te=(await estDetServices.lookUpTe(ed))/100.0;
        }       
        return est;
    }

    public EstimateV2 CalcTasaEstad061(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {                                               
            ed.TasaEstad061=estDetServices.CalcTasaEstad061(ed);
        }
        return est;
    }  

    public EstimateV2 CalcBaseGcias(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {                                               
            ed.BaseIvaGcias=estDetServices.CalcBaseIvaGcias(ed);
        }
        return est;
    }  

    public async Task<EstimateV2> searchIva(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.IVA=(await estDetServices.lookUpIVA(ed))/100.0;
        }       
        return est;
    }

    public EstimateV2 CalcIVA415(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.IVA415=estDetServices.CalcIVA415(ed);
        }
        return est;
    }

    public async Task<EstimateV2> searchIvaAdic(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.IVA_ad=(await estDetServices.lookUpIVAadic(ed))/100.0;
        }
        return est;
    } 

    public EstimateV2 CalcIVA_ad_Gcias(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.IVA_ad_gcias=estDetServices.CalcIvaAdic(ed,est.IvaExcento);
        }
        return est;
    }

    public EstimateV2 CalcImpGcias424(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.ImpGcias424=estDetServices.CalcImpGcias424(ed);
        }
        return est;
    }

    public async Task<EstimateV2> CalcIIBB900(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.IIBB=await estDetServices.CalcIIBB(ed);
        }
        return est;
    }

    public EstimateV2 CalcPrecioUnitUSS(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.PrecioUnitUSS=estDetServices.CalcPrecioUnitUSS(ed);
        }
        return est;
    }

    public EstimateV2 CalcPagado(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.Pagado=estDetServices.CalcPagado(ed);
        }
        return est;
    }

    public EstimateV2 CalcFactorProdTotal(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.FactorProd=estDetServices.CalcFactorProducto(ed, sumFobTotal(est));       
        }
        return est;
    }

    public double sumPesoTotal(EstimateV2 est)
    {
        double tmp=0;
        foreach(EstimateDetail ed in est.EstDetails)
        {
            tmp+=ed.PesoTot;
        }
        return tmp;
    }

    public double sumFobTotal(EstimateV2 est)
    {
        double tmp=0;
        foreach(EstimateDetail ed in est.EstDetails)
        {
            tmp+=ed.Fob;
        }
        return tmp;
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

}