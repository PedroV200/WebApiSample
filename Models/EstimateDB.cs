namespace WebApiSample.Models;
using WebApiSample.Core;
using WebApiSample.Infrastructure;
public class EstimateDB
{
    public EstimateHeaderDB estHeader {get; set;}
    public List<EstimateDetailDB> estDetails {get; set;}
    
}