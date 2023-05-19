namespace WebApiSample.Models;

public class EstimateDetailDB
{
    // VALORES DADOS
    public int Id {get; set;}
    public string Modelo {get; set;}
    public string Ncm {get; set;}
    public double Pesounitxcaja {get; set;}
    public double Cbmxcaja {get; set; }
    public int Pcsxcaja {get; set;}
    public double Fobunit {get; set;}
    public int Cantpcs {get; set;}
    public int IdEstHeader { get; set; }
}