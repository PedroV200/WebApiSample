namespace WebApiSample.Models;
using WebApiSample.Core;
using WebApiSample.Infrastructure;
public class Estimate
{
    public EstimateHeader estHeader {get; set;}
    public List<EstimateDetail> estDetails {get; set;}
    
}