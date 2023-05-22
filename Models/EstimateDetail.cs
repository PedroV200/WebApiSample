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
    
    // SIGUE ......

}