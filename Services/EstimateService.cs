namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class EstimateService: IEstimateService
{
    //IUnitOfWork _unitOfWork;
    public EstimateService(IEstimateDetailService estDetailServices/*,IUnitOfWork unitOfWork*/)
    {
        estDetServices=estDetailServices;
       // _unitOfWork=unitOfWork;

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

    public EstimateV2 CalcFactorProdTotal(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.PesoTot=estDetServices.CalcFactorProducto(ed, sumFobTotal(est));       
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
            ed.Die=await estDetServices.lookUpDie(ed);
        }
        return est;
    }

    public async Task<EstimateV2> resolveNcmTe(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.Te=await estDetServices.lookUpTe(ed);
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

}