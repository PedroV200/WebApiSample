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
    // Conforme COL V
    public double CalcTasaEstad061(EstimateDetail est)
    {
        if(est.Te==0)
        {
            return 0;
        }
        else
        {
            if(est.valAduanaDivisa<10000)
            {
                if((est.valAduanaDivisa*est.Te)>180)
                {
                    return 180;
                }
                else
                {
                    return est.valAduanaDivisa*est.Te;
                }
            }
            else
            {
                return est.valAduanaDivisa*est.Te;
            }
        }
    }

    public double CalcBaseIvaGcias(EstimateDetail estD)
    {
         return (estD.valAduanaDivisa+estD.Derechos+estD.TasaEstad061);
    }   


    public async Task<double> lookUpIVA(EstimateDetail estDetails)
    {
            NCM myNCM=await _unitOfWork.NCMs.GetByIdStrAsync(estDetails.ncm); 
            // VER COLUMNA U (U15 en adelante).
            // Existe el te ?. No puedo tener un te "EN BLANCO" como el XLS. Lo ideal que x defecto tengan un valor negativo
            // como para indicar que esta "en blanco".
            if(myNCM!=null)
            { // Si.
                return myNCM.iva;   
            }            
            return -1;
    } 

    public double CalcIVA415(EstimateDetail estDetail)
    {
        return estDetail.BaseIvaGcias*estDetail.IVA;
    }

    public async Task<double> lookUpIVAadic(EstimateDetail estDetails)
    {
            NCM myNCM=await _unitOfWork.NCMs.GetByIdStrAsync(estDetails.ncm); 
            // VER COLUMNA U (U15 en adelante).
            // Existe el te ?. No puedo tener un te "EN BLANCO" como el XLS. Lo ideal que x defecto tengan un valor negativo
            // como para indicar que esta "en blanco".
            if(myNCM!=null)
            { // Si.
                return myNCM.iva_ad;   
            }            
            return -1;
    } 

    public double CalcIvaAdic(EstimateDetail estDetails, bool ivaEx)
    {
        if(ivaEx)
        {
            return 0;
        }
        else
        {
            return estDetails.BaseIvaGcias*estDetails.IVA_ad;
        }
    }

    public double CalcImpGcias424(EstimateDetail estDetails)
    {
        return (estDetails.BaseIvaGcias*6)/100;
    }

    public double CalcIIBB(EstimateDetail estDetails)
    {
        // ADVERTENCIA: Es la suma de todos los valores de IIBB de la tabla IIBB, CELDA D26
        // IMPLEMENTAR metodo que devuelve la suma usnado query.
        double totalIIBB=2.8429;
        return (estDetails.BaseIvaGcias*totalIIBB);
    }

    public double CalcPrecioUnitUSS(EstimateDetail estDetails)
    {
        if(estDetails.cantpcs>0)
        {
            return estDetails.BaseIvaGcias/estDetails.cantpcs;
        }
        else
        {
            return -1;
        }
    }

    public double CalcPagado(EstimateDetail estDetails)
    {
        return(estDetails.IIBB+estDetails.ImpGcias424+estDetails.IVA_ad+estDetails.TasaEstad061+estDetails.Derechos);
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