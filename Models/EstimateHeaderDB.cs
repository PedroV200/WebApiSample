namespace WebApiSample.Models;
public class EstimateHeaderDB
{ // LISTED 26_7_2023 17:05
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
    // Condicion ante el iva    CELDA I5
    public bool IvaExcento {get; set;}
    // CELDA F3
             
    // Tipo de contenedor: 40HQ / 40ST / 20ST / LCL   CELDA C9
    public string FreightType {get; set; }
    // Foward from: CHINA / PANAMA                    VAR LIBRO BASE_TARIFAS
    // Entre el FreightType y el Freightfwd se puede determinar el costo del flete.
    public string FreightFwd {get;set;}
    // Seguro: Un porcentaje sobre el FOBT TOTAL (lasumatoria de todos los fobs del detalle)
    // USADO EN CALCULO CELDA C5
    
    
    // Celda C3
    public double FobGrandTotal {get;set;}
    // Celda C4
    public double FleteTotal {get;set;}
    // El valor magico "0.1 en la formula de C5
    //public double SeguroPorct {get;set;}
    // CELDA C5
    public double Seguro{get; set;}
    // CELDA C7 
    //public double ArancelSim{get;set;}
    // CELDA C10
    public double CantidadContenedores{get;set;}
    // CELDA I3 (sumatoria de los AF en AF43)
    public double Pagado{get;set;}

    public double CbmTot {get;set;}
    public double CifTot {get;set;}
    public double IibbTot {get;set;}
    public double pesoTotal {get;set;}
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
    public double c_seguroporct{get;set;}
    public double c_arancelsim{get;set;}
    public double DolarBillete{get;set;}    
    public string PolizaProv {get;set;}
    public string p_gloc_banco{get;set;}
    public string p_gloc_fwder{get;set;}
    public string p_gloc_term{get;set;}
    public string p_gloc_despa{get;set;}
    public string p_gloc_tte{get;set;}
    public string p_gloc_cust{get;set;}
    public string p_gloc_gestdigdoc{get;set;}
    // Provvedores: CELDA E6 a E13
    public string oemprove1{get;set;}
    public string oemprove2{get;set;}
    public string oemprove3{get;set;}
    public string oemprove4{get;set;}
    public string oemprove5{get;set;}
    public string oemprove6{get;set;}
    public string oemprove7{get;set;}
    public int id_PolizaProv{get;set;}
    public int id_p_gloc_banco{get;set;}
    public int id_p_gloc_fwder{get;set;}
    public int id_p_gloc_term{get;set;}
    public int id_p_gloc_despa{get;set;}
    public int id_p_gloc_tte{get;set;}
    public int id_p_gloc_cust{get;set;}
    public int id_p_gloc_gestdigdoc{get;set;}
    public int id_oemprove1{get;set;}
    public int id_oemprove2{get;set;}
    public int id_oemprove3{get;set;}
    public int id_oemprove4{get;set;}
    public int id_oemprove5{get;set;}
    public int id_oemprove6{get;set;}
    public int id_oemprove7{get;set;}

   

    // Momento de la emision
    public DateTime hTimeStamp {get; set;}


}