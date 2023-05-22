namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class EstimateDetailService: IEstimateDetailService
{
    //public IEstimateDetailService estDetServ {get;}

    //public EstimateDetailService(IEstimateDetailService estDetService)
    //{
    //    estDetServ=estDetService;
    //}

    public double CalcPesoTotal(EstimateDetail estD)
    {
        if(estD.cantpcs!=0)
        {
            return (estD.pesounitxcaja/estD.pcsxcaja)*estD.cantpcs;
        }
        else
        {
            return -1;
        }
    }

    public double CalcFob(EstimateDetail estD)
    {
        return (estD.cantpcs*estD.fobunit);
    }

    public double CalcFactorProducto(EstimateDetail estD, double fobTotal)
    {
        if(fobTotal!=0)
        {
            return (estD.fobunit/fobTotal);
        }
        else
        {
            return -1;
        }
    }
}