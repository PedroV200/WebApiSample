namespace WebApiSample.Models;
using WebApiSample.Core;
using WebApiSample.Infrastructure;
public class EstimateDB
{
    public EstimateHeaderDB estHeaderDB {get; set;}
    public List<EstimateDetailDB> estDetailsDB {get; set;}
    
}