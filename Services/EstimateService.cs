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
            ed.PesoTot=estDetServices.CalcFob(ed);       
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