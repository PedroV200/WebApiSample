namespace WebApiSample.Models;

public class EstimateDetail
{
    public int id {get; set;}
    public string modelo {get; set;}
    public string ncm {get; set;}
    public double pesounitxcaja {get; set;}
    public double cbmxcaja {get; set; }
    public int pcsxcaja {get; set;}
    public double fobunit {get; set;}
    public int cantpcs {get; set;}
    public int code { get; set; }
}