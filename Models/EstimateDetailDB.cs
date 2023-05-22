namespace WebApiSample.Models;

public class EstimateDetailDB
{
    // VALORES DADOS
    public int Id {get; set;}
    public string Modelo {get; set;}
    public string Ncm {get; set;}
    public double PesoUnitxCaja {get; set;}
    public double CbmxCaja {get; set; }
    public int PcsxCaja {get; set;}
    public double FobUnit {get; set;}
    public int CantPcs {get; set;}
    public int IdEstHeader { get; set; }
}