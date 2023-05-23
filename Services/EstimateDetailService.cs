namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class EstimateDetailService: IEstimateDetailService
{
    public IUnitOfWork _unitOfWork {get;}

    public EstimateDetailService(IUnitOfWork unitOfWork)
    {
        _unitOfWork=unitOfWork;
    }

    public async Task<double> lookUpDie(EstimateDetail estDetails)
    {
        NCM myNCM=await _unitOfWork.NCMs.GetByIdStrAsync(estDetails.ncm); 
        if(myNCM!=null)
        {
            return myNCM.die;
        }   
        return -1;
    }

    public async Task<double> lookUpTe(EstimateDetail estDetails)
    {
            double tmpL;
            NCM myNCM=await _unitOfWork.NCMs.GetByIdStrAsync(estDetails.ncm); 
            // VER COLUMNA U (U15 en adelante).
            // Existe el te ?. No puedo tener un te "EN BLANCO" como el XLS. Lo ideal que x defecto tengan un valor negativo
            // como para indicar que esta "en blanco".
            if(myNCM!=null && myNCM.te!>=0)
            { // Si.
                tmpL=myNCM.te;   
            }
            else
            { // No, entonces vale 3%.
                tmpL=3.00000;
            }
            return tmpL;
    }

    public double CalcPesoTotal(EstimateDetail estD)
    {
        if(estD.cantpcs!=0)
        {
            return (estD.pesounitxcaja/estD.pcsxcaja)*estD.cantpcs;
        }
        return -1;
    }

    public double CalcCbmTotal(EstimateDetail estD)
    {
        if(estD.pcsxcaja!=0)
        {
            return (estD.cantpcs*estD.cbmxcaja)/estD.pcsxcaja;
        }
        return -1;
    }

    public double CalcFob(EstimateDetail estD)
    {
        return (estD.cantpcs*estD.fobunit);
    }

    public double CalcFlete(EstimateDetail estD, double costoFlete,double fobGrandTotal)
    {
        if(fobGrandTotal>0)
        {
            return (estD.Fob/fobGrandTotal)*costoFlete;
        }
        return -1;
    }

    public double CalcSeguro(EstimateDetail estD, double seguroTotal, double fobGrandTotal)
    {
        if(fobGrandTotal>0)
        {
            return ((estD.Fob/fobGrandTotal)*seguroTotal);
        }
        return -1;
    }

    public double CalcValorEnAduanaDivisa(EstimateDetail estD)
    {
        return estD.Cif;
    }

    public double CalcCif(EstimateDetail estD)
    {
         return(estD.Seguro+estD.Flete+estD.Fob);
    }

    public double CalcDerechos(EstimateDetail est)
    {
        return est.valAduanaDivisa*est.Die;
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