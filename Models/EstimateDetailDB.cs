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
    // Agregado 14_06_2023
    public double ncm_die { get; set; }
    public double ncm_te { get; set; }
    public double ncm_iva { get; set; }
    public double ncm_ivaad { get; set; }
    public double sparef1 { get; set; }
    public double sparef2 { get; set; }
    public double sparef3 { get; set; }
    public double sparef4 { get; set; }

    

} 