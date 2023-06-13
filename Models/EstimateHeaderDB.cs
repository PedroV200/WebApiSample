namespace WebApiSample.Models;
public class EstimateHeaderDB
{
    // Id unico autoincremental de la BD (PK)
    public int Id { get; set; }
    // Spare
    public string Description { get; set; }
    // Numero de Estimate
    public int EstNumber { get; set; }
    // Version del Estimate
    public int EstVers { get; set; }
    // Emitido por:
    public string Own { get; set;}
    // Categoria de Articulo    SOLPA DEL LIBRO
    public string ArticleFamily {get; set;}
    // Proveedor                CELDA E6-E13
    public string OemSupplier {get; set; }
    // Condicion ante el iva    CELDA I5
    public bool IvaExcento {get; set;}
    // CELDA F3
    public double DollarBillete{get;set;}             
    // Tipo de contenedor: 40HQ / 40ST / 20ST / LCL   CELDA C9
    public string FreightType {get; set; }
    // Foward from: CHINA / PANAMA                    VAR LIBRO BASE_TARIFAS
    // Entre el FreightType y el Freightfwd se puede determinar el costo del flete.
    public string FreightFwd {get;set;}
    // Seguro: Un porcentaje sobre el FOBT TOTAL (lasumatoria de todos los fobs del detalle)
    // USADO EN CALCULO CELDA C5
    
    // Momento de la emision
    public DateTime hTimeStamp {get; set;}
    // Celda C3
    public double FobGrandTotal {get;set;}
    // Celda C4
    public double FleteTotal {get;set;}
    // El valor magico "0.1 en la formula de C5
    public double SeguroPorct {get;set;}
    // CELDA C5
    public double Seguro{get; set;}
    // CELDA C10
    public double CantidadContenedores{get;set;}
    // CELDA I3 (sumatoria de los AF en AF43)
    public double Pagado{get;set;}

    public double CbmTot {get;set;}
    public double CifTot {get;set;}
    public string PolizaProv {get;set;}
    public double ExtraGastosLocProyectado {get;set;}

    // CONSTANTES
    public double c_gdespa_cif_min{get;set;}
    public double c_gdespa_cif_mult{get;set;}
    public double c_gdespa_cif_thrhld{get;set;}
    public double c_gcust_thrshld{get;set;}
    public double c_ggesdoc_mult{get;set;}
    public double c_gbanc_mult{get;set;}
    public double c_ncmdie_min{get;set;}  
    public double c_est061_thrhldmax{get;set;}
    public double c_est061_thrhldmin{get;set;}
    public double c_gcias424_mult{get;set;}

    public string p_gloc_banco{get;set;}
    public string p_gloc_fwder{get;set;}
    public string p_gloc_term{get;set;}
    public string p_gloc_despa{get;set;}
    public string p_gloc_tte{get;set;}
    public string p_gloc_cust{get;set;}
    public string p_gloc_gestdigdoc{get;set;}

}