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

    public ICnstService _constService;

    public CONSTANTES misConsts=new CONSTANTES();

    public EstimateDetailService(IUnitOfWork unitOfWork, ICnstService constService)
    {
        _unitOfWork=unitOfWork;
        _constService=constService;
    }

    public async void loadConstants(CONSTANTES miaConst)
    {
        misConsts=miaConst;
    }


    public async Task<NCM> lookUp_NCM_Data(EstimateDetail estDetails)
    {
        NCM myNCM=await _unitOfWork.NCMs.GetByIdStrAsync(estDetails.ncm); 
        if(myNCM!=null)
        {
            return myNCM;
        }   
        return null;
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
                //tmpL=3.00000;
                tmpL=misConsts.CONST_NCM_DIE_Min;
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
        return est.valAduanaDivisa*est.ncm_die;
    }
    // Conforme COL V
    public double CalcTasaEstad061(EstimateDetail est)
    {
        if(est.ncm_te==0)
        {
            return 0;
        }
        else
        {
            if(est.valAduanaDivisa<misConsts.CNST_ESTAD061_ThrhldMAX)
            //if(est.valAduanaDivisa<10000)
            {
                if((est.valAduanaDivisa*est.ncm_te)>misConsts.CNST_ESTAD061_ThrhldMIN)
                //if((est.valAduanaDivisa*est.Te)>180)
                {
                    //return 180;
                    return misConsts.CNST_ESTAD061_ThrhldMIN;
                }
                else
                {
                    return est.valAduanaDivisa*est.ncm_te;
                }
            }
            else
            {
                return est.valAduanaDivisa*est.ncm_te;
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
        return estDetail.BaseIvaGcias*estDetail.ncm_iva;
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
            return estDetails.BaseIvaGcias*estDetails.ncm_ivaad;
        }
    }

    public double CalcImpGcias424(EstimateDetail estDetails)
    {
        //return (estDetails.BaseIvaGcias*6)/100;
        return estDetails.BaseIvaGcias*misConsts.CNST_GCIAS_424_Mult;
    }

    public double CalcIIBB(EstimateDetail estDetails, double sumaFactoresIIBB)
    {
        //double totalIIBB=await _unitOfWork.IIBBs.GetSumFactores();
        return (estDetails.BaseIvaGcias*(sumaFactoresIIBB/100));
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
        return(estDetails.IIBB+estDetails.ImpGcias424+estDetails.IVA_ad_gcias+estDetails.IVA415+estDetails.TasaEstad061+estDetails.Derechos);
    }

    

    public double CalcFactorProducto(EstimateDetail estD, double fobTotal)
    {
        if(fobTotal!=0)
        {
            return (estD.Fob/fobTotal);
        }
        else
        {
            return -1;
        }
    }

    public double CalcGastosProyPond(EstimateDetail estD, double gastosTotProy)
    {
        return estD.FactorProd*gastosTotProy;
    }
    public double CalcGastosProyPondUSS(EstimateDetail estD,double dolar)
    {
        return estD.ExtraGastoLocProy/dolar;
    }
    public double CalcGastosProyPorUnidUSS(EstimateDetail estD)
    {
        if(estD.cantpcs>0)
        {
            return estD.ExtraGastoLocProyUSS/estD.cantpcs;
        }
        else
        {
            return -1;
        }
    }
    public double CalcOverHeadUnitUSS(EstimateDetail estD)
    {
        if(estD.PrecioUnitUSS!=0)
        {
            return estD.ExtraGastoLocProyUnitUSS/estD.PrecioUnitUSS;
        }
        else
        {
            return -1;
        }
    }

    public double CalcCostoUnitUSS(EstimateDetail estD)
    {
        return estD.PrecioUnitUSS+estD.ExtraGastoLocProyUnitUSS;
    }

    public double CalcCostoUnit(EstimateDetail estD, double dolar)
    {
        return estD.CostoUnitEstimadoUSS*dolar;
    }
}