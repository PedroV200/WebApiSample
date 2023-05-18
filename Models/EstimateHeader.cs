namespace WebApiSample.Models;
public class EstimateHeader
{
    public int code { get; set; }
    public string own { get; set;}
    public string description { get; set; }
    public string articlefamily {get; set;}
    public string oemsupplier {get; set; }
    public bool ivaexcento {get; set;}
    public double dollarBillete{get;set;}
    public string freighttype {get; set; }
    public string freightfwd {get;set;}
    public double seguro {get;set;}
    public string htimestamp {get; set;}
}