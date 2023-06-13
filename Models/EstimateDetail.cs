namespace WebApiSample.Models;

public class EstimateDetail
{
    // VALORES DADOS
    public int id {get; set;}
    public string modelo {get; set;}
    public string ncm {get; set;}
    public double pesounitxcaja {get; set;}
    public double cbmxcaja {get; set; }
    public int pcsxcaja {get; set;}
    public double fobunit {get; set;}
    public int cantpcs {get; set;}
    public int idestheader { get; set; }
    // VALORES CALCULADOS
    public double PesoTot {get;set;}
    public double CbmTot {get;set;}
    public double Fob {get;set;}
    public double Flete {get;set;}
    public double Seguro {get;set;}
    public double Cif {get; set;}
    public double valAduanaDivisa {get; set;}
    // Die = Derechos Importaciones
    public double Die {get; set;}
    public double Derechos {get;set;}
    public double Te {get;set;}
    public double TasaEstad061 {get;set;}
    public double BaseIvaGcias {get;set;}
    public double IVA {get;set;}
    public double IVA415 {get;set;}
    public double IVA_ad {get;set;}
    public double IVA_ad_gcias {get; set;}    
    public double ImpGcias424 {get;set;}
    public double IIBB {get;set;}
    public double PrecioUnitUSS {get;set;}
    public double Pagado {get;set;}
    public double FactorProd {get;set;}
    public double ExtraGastoLocProy {get;set;}
    public double ExtraGastoLocProyUSS {get;set;}
    public double ExtraGastoLocProyUnitUSS {get;set;}
    public double OverHead {get;set;}
    public double CostoUnitEstimadoUSS {get;set;}
    public double CostoUnitEstimado {get;set;}
}