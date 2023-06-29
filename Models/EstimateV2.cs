namespace WebApiSample.Models;
// NOTA: comentarios de celdas segun presup. ARG, libro "N - Duchas Escocesas"
// ADVERTENCIA: No esta mapeada la tabla "comprobantes de pago / gastos locales"
public class EstimateV2
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
    public string Owner { get; set;}
    // Categoria de Articulo    SOLPA DEL LIBRO
    public string ArticleFamily {get; set;}
    // Condicion ante el iva    CELDA I5
    public bool IvaExcento {get; set;}
    // CELDA F3
    public double DolarBillete{get;set;}             
    // Tipo de contenedor: 40HQ / 40ST / 20ST / LCL   CELDA C9
    public string FreightType {get; set; }
    // Foward from: CHINA / PANAMA                    VAR LIBRO BASE_TARIFAS
    // Entre el FreightType y el Freightfwd se puede determinar el costo del flete.
    public string FreightFwd {get;set;}
    // Seguro: Un porcentaje sobre el FOBT TOTAL (lasumatoria de todos los fobs del detalle)
    // USADO EN CALCULO CELDA C5
    
    // Momento de la emision
    public DateTime TimeStamp {get; set;}
    // Celda C3
    public double FobGrandTotal {get;set;}
    // Celda C4
    public double FleteTotal {get;set;}
    // El valor magico "0.1 en la formula de C5
    //public double SeguroPorct {get;set;}
    // CELDA C5
    public double Seguro{get; set;}
    // CELDA C7 
    // public double ArancelSim{get;set;}
    // CELDA C10
    public double CantidadContenedores{get;set;}
    // CELDA I3 (sumatoria de los AF en AF43)
    public double Pagado{get;set;}
    // Lista de los diferentes items con sus valores dados del Estimate.


    // REFACTOR DEL HEADER --------------- 
    //######################################
    public double CbmGrandTot {get;set;}
    public double CifTot {get;set;}

    public double IibbTot {get;set;}
    public string PolizaProv {get;set;}
    public double ExtraGastosLocProyectado {get;set;}
    public CONSTANTES constantes{get;set;}

    public string p_gloc_banco{get;set;} 
    public string p_gloc_fwder{get;set;}  
    public string p_gloc_term{get;set;}  
    public string p_gloc_despa{get;set;}  
    public string p_gloc_tte{get;set;}  
    public string p_gloc_cust{get;set;}  
    public string p_gloc_gestdigdoc{get;set;} 	
	public string oemprove1{get;set;} 
    public string oemprove2{get;set;}
    public string oemprove3{get;set;}
    public string oemprove4{get;set;}
    public string oemprove5{get;set;}
    public string oemprove6{get;set;}
    public string oemprove7{get;set;}

    //######################################
    public List<EstimateDetail> EstDetails {get; set;}


    public EstimateV2()
    {
        this.EstDetails=new List<EstimateDetail>();
        this.constantes=new CONSTANTES();
    }
}