namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

// LISTED 29_6_2023 14:48

public class EstimateService: IEstimateService
{
    IUnitOfWork _unitOfWork;

    string haltError;
    public EstimateService(IEstimateDetailService estDetailServices,IUnitOfWork unitOfWork, ICnstService constService)
    {
       _estDetServices=estDetailServices;
       _unitOfWork=unitOfWork;
       _cnstService=constService;

    }
    public IEstimateDetailService _estDetServices {get;}
    public ICnstService _cnstService {get;}

// ADVERTENCIA: Esta funcion es equi a init(), debe llamarse antes que cualquier cuenta de modo que las constantes
// esten todas populadas.
// Se encarga de obtener las constantes que se usan en diverso calculos desde la tabla constantes.
// Le pasa estas constantes tmb a estDetailService.
    public async Task<EstimateV2> loadConstants(EstimateV2 est)
    {
        est.constantes=await _cnstService.getConstantes();
        if(est.constantes==null)
        {
            return null;
        }
        // Le paso las constantes a estDetailService tmb.
        _estDetServices.loadConstants(est.constantes);
        return est;
        
    }

    public EstimateV2 CalcPesoTotal(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.PesoTot=_estDetServices.CalcPesoTotal(ed); 
            if(ed.PesoTot<0)
            {
                haltError=$"ATENCION: El articulo modelo '{ed.modelo}' tiene cant pcs x caja = 0. DIV 0 !";
                return null;
            }      
        }
        return est;
    }



    public EstimateV2 CalcFobTotal(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.Fob=_estDetServices.CalcFob(ed);       
        }
        return est;
    }

    public EstimateV2 CalcCbmTotal(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.CbmTot=_estDetServices.CalcCbmTotal(ed);   
            if(ed.CbmTot<0)
            {
                haltError=$"ATENCION: El articulo '{ed.modelo}' tiene cant pcs por caja = 0. DIV 0 !";
                return null;
            }    
        }
        return est;
    }

    public EstimateV2 CalcFleteTotalByProd(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.Flete=_estDetServices.CalcFlete(ed,est.FleteTotal,est.FobGrandTotal); 
            if(ed.Flete<0)
            {
                return null;
            }      
        }
        return est;
    }

    public EstimateV2 CalcSeguro(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {                                               // CELDA C5=0.1*C3
            ed.Seguro=_estDetServices.CalcSeguro(ed,est.Seguro,est.FobGrandTotal);   
            if(ed.Seguro<0)
            {
                return null;
            }    
        }
        return est;
    }

    public EstimateV2 CalcCif(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {                                               
            ed.Cif=_estDetServices.CalcCif(ed);
        }
        return est;
    }

    // En las columnas P y Q no se hace nada actualmente. 
    // El resultado de los ajsutes es la columna R designada como ValorAduanaEndivisa
    public EstimateV2 CalcAjusteIncDec(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {                                               
            ed.valAduanaDivisa=_estDetServices.CalcValorEnAduanaDivisa(ed);
        }
        return est;
    }

    public async Task<EstimateV2> searchNcmDie(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {                                               
            ed.ncm_die=(await _estDetServices.lookUpDie(ed))/100.0;
        }
        return est;
    }

    public EstimateV2 CalcDerechos(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {                                               
            ed.Derechos=_estDetServices.CalcDerechos(ed);
        }
        return est;
    }  

    public async Task<EstimateV2> resolveNcmTe(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.ncm_te=(await _estDetServices.lookUpTe(ed))/100.0;
        }       
        return est;
    }

    public EstimateV2 CalcTasaEstad061(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {                                               
            ed.TasaEstad061=_estDetServices.CalcTasaEstad061(ed);
        }
        return est;
    }  

    public EstimateV2 CalcBaseGcias(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {                                               
            ed.BaseIvaGcias=_estDetServices.CalcBaseIvaGcias(ed);
        }
        return est;
    }  

    public async Task<EstimateV2> searchIva(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.ncm_iva=(await _estDetServices.lookUpIVA(ed))/100.0;
        }       
        return est;
    }

    public EstimateV2 CalcIVA415(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.IVA415=_estDetServices.CalcIVA415(ed);
        }
        return est;
    }

    public async Task<EstimateV2> searchIvaAdic(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.ncm_ivaad=(await _estDetServices.lookUpIVAadic(ed))/100.0;
        }
        return est;
    }

     public async Task<EstimateV2> search_NCM_DATA(EstimateV2 est)
    {
        NCM myNCM=new NCM();

        foreach(EstimateDetail ed in est.EstDetails)
        {  
           myNCM=await _estDetServices.lookUp_NCM_Data(ed); 
           if(myNCM==null)
           {    // Logeo que NCM / Articulo fallo
                haltError=$"FALLO NCM='{ed.ncm}', DET= '{ed.modelo}";
                return null;
           }
           ed.ncm_die=myNCM.die/100.0;
           ed.ncm_te=myNCM.te/100.0;
           ed.ncm_iva=myNCM.iva/100.0;
           ed.ncm_ivaad=myNCM.iva_ad/100.0; 
        }
        return est;
    }

    public EstimateV2 CalcIVA_ad_Gcias(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.IVA_ad_gcias=_estDetServices.CalcIvaAdic(ed,est.IvaExcento);
        }
        return est;
    }

    public EstimateV2 CalcImpGcias424(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.ImpGcias424=_estDetServices.CalcImpGcias424(ed);
        }
        return est;
    }

    public EstimateV2 CalcIIBB900(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.IIBB=_estDetServices.CalcIIBB(ed, est.IibbTot);
        }
        return est;
    }

    public EstimateV2 CalcPrecioUnitUSS(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.PrecioUnitUSS=_estDetServices.CalcPrecioUnitUSS(ed);
            if(ed.PrecioUnitUSS<0)
            {
                haltError=$"ATENCION: Articulo '{ed.modelo}' tiene can pcs = 0. DIV 0 !";
                return null;
            }
        }
        return est;
    }

    public EstimateV2 CalcPagado(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.Pagado=_estDetServices.CalcPagado(ed);
        }
        return est;
    }

    public EstimateV2 CalcFactorProdTotal(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.FactorProd=_estDetServices.CalcFactorProducto(ed,est.FobGrandTotal);   
            if(ed.FactorProd<0)
            {
                haltError=$"ATENCION: El articulo '{ed.modelo}' tiene un FOB TOT de 0. Div 0!";
                return null;
            }    
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

    public EstimateV2 CalcCbmGrandTotal(EstimateV2 est)
    {
        double tmp=0;
        foreach(EstimateDetail ed in est.EstDetails)
        {
            tmp+=ed.CbmTot;
        }
        est.CbmGrandTot=tmp;
        return est;
    }

    public EstimateV2 CalcCifTotal(EstimateV2 est)
    {
        double tmp=0;
        foreach(EstimateDetail ed in est.EstDetails)
        {
            tmp+=ed.Cif;
        }
        est.CifTot=tmp;
        return est;
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


    public async Task<double> calcularGastosFwd(EstimateV2 miEst)
    {
        TarifasFwdCont myContFwd=new TarifasFwdCont();
        
        if(miEst!=null && miEst.FreightType!=null && miEst.FreightFwd!=null)
        {
            myContFwd= await _unitOfWork.TarifasFwdContenedores.GetByFwdContTypeAsync(miEst.FreightFwd,miEst.FreightType);
        }
        else
        {   // El estimate pasado como parametro o alguno de sus miembros es null !!!. OJO.
            return -1;
        }

        if(myContFwd==null)
        {
            return -1;
        }

        
        if(miEst.FreightType=="LCL")
        {
            return (myContFwd.costoflete040*miEst.CbmGrandTot*miEst.DolarBillete)+myContFwd.gastos1*miEst.DolarBillete;
        }
        else
        {
            return (myContFwd.costoflete040+myContFwd.gastos1)*miEst.DolarBillete*miEst.CantidadContenedores;
        }
    }

    public async Task<double> calcularGastosTerminal(EstimateV2 miEst)
    {
        TarifasTerminal myTar=new TarifasTerminal();
        if(miEst==null || miEst.FreightType==null)
        {
            return -1;
        }
        myTar= await _unitOfWork.TarifasTerminals.GetByContTypeAsync(miEst.FreightType);
        if(myTar==null)
        {
            return -1;
        }
        return ((myTar.gastoFijo+myTar.gastoVariable)*miEst.DolarBillete*miEst.CantidadContenedores);
    }

    public async Task<double> calcularGastosDespachante(EstimateV2 miEst)
    {
        double tmp;
        if(miEst==null)
        {   // OJO con que me pasen NULL.   
            return -1;
        }

        if((miEst.CifTot*miEst.constantes.CNST_GASTOS_DESPA_Cif_Mult)>miEst.constantes.CNST_GASTOS_DESPA_Cif_Thrhld)
        {
            tmp=miEst.CifTot*miEst.constantes.CNST_GASTOS_DESPA_Cif_Mult*miEst.DolarBillete;
        }
        else
        {
            tmp=miEst.DolarBillete*miEst.constantes.CNST_GASTOS_DESPA_Cif_Min;
        }

        return tmp+(miEst.constantes.CNST_GASTOS_DESPA_Cif_Thrhld*miEst.DolarBillete);
    }

    public async Task<double> calcularGastosTteLocal(EstimateV2 miEst)
    {
        double tmp;
        if(miEst==null || miEst.FreightType==null)
        {
            return -1;
        }

        TarifasTteLocal myTar= await _unitOfWork.TarifasTtesLocal.GetTarifaTteByContAsync(miEst.FreightType);

        if(myTar==null)
        {
            return -1;
        }
        // Calculos los gastos totales de transporte. Aun cuando tenga un campo gastostot.
        tmp=myTar.fleteint+myTar.devacio+myTar.demora+myTar.guarderia;
        // si es un LCL, es menos que un contenedor. No se multiplica por Cantidad de Contenedores.
        if(miEst.FreightType=="LCL")
        {
            return tmp;
        }
        else
        {
            return tmp*miEst.CantidadContenedores;
        }
    }

    public async Task<double> calcularGastosCustodia(EstimateV2 miEst)
    {
            TarifasPoliza myTar=new TarifasPoliza();

            if(miEst==null)
            {
                return -1;
            }

            myTar=await _unitOfWork.TarifasPolizas.GetByDescAsync(miEst.PolizaProv);

            if(myTar==null)
            {
                return -1;
            }

            if(miEst.FobGrandTotal>miEst.constantes.CNST_GASTOS_CUSTODIA_Thrshld)
            {
                return (myTar.prima+myTar.demora) + ((myTar.prima+myTar.demora)*(myTar.impint/100)*(myTar.sellos/100));
            }
            else
            {
                return 0;
            }
    }

    public double calcularGastosGestDigDocs(EstimateV2 miEst)
    {
        return miEst.constantes.CNST_GASTOS_GESTDIGDOC_Mult*miEst.DolarBillete;
    }

    public double calcularGastosBancarios(EstimateV2 miEst)
    {
        return miEst.constantes.CNST_GASTOS_BANCARIOS_Mult*miEst.DolarBillete;
    }

// Hace las cuentas de la tabla inferior del presupuestador, gastos locales / proyectados.
// Los devuelve en dolarbillete. CELDA D59
    public async Task<double> calcularGastosProyecto(EstimateV2 miEst)
    {
        double tmp;
        double result;

        tmp=await calcularGastosFwd(miEst);
        if(tmp<0)
        {   // Todos los metodos que consultan una tabla tienen opcion de devolver -1 si algo no salio bien.
            haltError="FALLA CALCULAR GASTOS FWD. TABLA TarifasFWD no accesible o no existen datos para el tipo de contenedor / origen indicados";
            return tmp;
        }
        result=tmp;
        tmp=await calcularGastosTerminal(miEst);
        if(tmp<0)
        {
            haltError="FALLA AL CALCULAR GASTOS DE TERMINAL. TAbla no accesible o no existen datos para el tipo de contenedor ingresado";
            return tmp;
        }
        result=result+tmp;
        tmp=await calcularGastosDespachante(miEst);
        if(tmp<0)
        {

            return tmp;
        }
        result=result+tmp;
        tmp=await calcularGastosTteLocal(miEst);
        if(tmp<0)
        {
            haltError="FALLA AL CALCULAR LOS GASTOS DE TTE LOC. Tabla de tarifa no accesible o no existen datos para el contenedor ingresado";
            return tmp;
        }
        result=result+tmp;
        tmp=await calcularGastosCustodia(miEst);

        if(tmp<0)
        {
            haltError="FALLO AL CALCULAR LOS GASTOS DE CUSTODIA. Tabla de tarifa no accesible o no existen datos para el Proveedor de Poliza ingresado";
            return tmp;
        }
        result=result+tmp;
        tmp=calcularGastosGestDigDocs(miEst);       // Este metodo no involucra una consulta a tabla, tmp np puede ser negativo
        result=result+tmp;
        tmp=calcularGastosBancarios(miEst);         // Este idem.
        result=result+tmp;

        return (result);

    }



    public EstimateV2 CalcExtraGastoLocProyecto(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.ExtraGastoLocProy=_estDetServices.CalcGastosProyPond(ed,est.ExtraGastosLocProyectado);       
        }
        return est;
    }

    public EstimateV2 CalcExtraGastoProyectoUSS(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.ExtraGastoLocProyUSS=_estDetServices.CalcGastosProyPondUSS(ed,est.DolarBillete);       
        }
        return est;
    }

    public EstimateV2 CalcExtraGastoProyectoUnitUSS(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.ExtraGastoLocProyUnitUSS=_estDetServices.CalcGastosProyPorUnidUSS(ed); 
            if(ed.ExtraGastoLocProyUnitUSS<0)
            {
                haltError=$"ATENCION: El articulo '{ed.modelo}' tiene cant de pcs = 0. DIV 0!";
                return null;
            }      
        }
        return est;
    }

    public EstimateV2 CalcOverhead(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.OverHead=_estDetServices.CalcOverHeadUnitUSS(ed);  
            if(ed.OverHead<0)
            {
                haltError=$"ATENCION: El articulo '{ed.modelo}' tiene un PRECIO USS UNIT de 0. Div 0 !";
                return null;
            }     
        }
        return est;
    }

    public EstimateV2 CalcCostoUnitarioUSS(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.CostoUnitEstimadoUSS=_estDetServices.CalcCostoUnitUSS(ed);       
        }
        return est;
    }

    public EstimateV2 CalcCostoUnitario(EstimateV2 est)
    {
        foreach(EstimateDetail ed in est.EstDetails)
        {
            ed.CostoUnitEstimado=_estDetServices.CalcCostoUnit(ed,est.DolarBillete);       
        }
        return est;
    } 

    public async Task<EstimateV2> CalcularCantContenedores(EstimateV2 est)
    {
        Contenedor myCont=new Contenedor();
        myCont=await _unitOfWork.Contenedores.GetByTipoContAsync(est.FreightType);
        if(myCont==null)        
        {
            return null;
        }
        if(myCont.volume>0 && myCont.weight>0)
        {
            //est.CantidadContenedores=est.CbmGrandTot/myCont.volume;
            if((est.CbmGrandTot/myCont.volume)>(est.pesoTotal/myCont.weight))
            {// Gana el volumen
                est.CantidadContenedores=est.CbmGrandTot/myCont.volume;
            }
            else
            {// Gan el peso.
                est.CantidadContenedores=est.pesoTotal/myCont.weight;
            }
        }
        else
        {
            return null;
        }
        return est;
    }

    public async Task<EstimateV2> CalcFleteTotal(EstimateV2 est)
    {
        double tmp;
        tmp=await lookUpTarifaFleteCont(est);
        if(tmp==-1)
        {
            return null;
        }
        tmp=tmp*est.CantidadContenedores;
        est.FleteTotal=tmp;
        return est;
    }

    public EstimateV2 CalcSeguroTotal(EstimateV2 miEst)
    {
        miEst.Seguro=(miEst.constantes.CNST_SEGURO_PORCT/100)*miEst.FobGrandTotal;
        return miEst;
    }

    public EstimateV2 CalcPagadoTot(EstimateV2 miEst)
    {
        double tmp=0;
        foreach(EstimateDetail ed in miEst.EstDetails)
        {
            tmp+=ed.Pagado;
        }
        miEst.Pagado=tmp+miEst.constantes.CNST_ARANCEL_SIM;
        return miEst;
    }

    public string getLastError()
    {
        return haltError;
    }

}