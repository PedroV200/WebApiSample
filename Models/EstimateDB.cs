namespace WebApiSample.Models;
using WebApiSample.Core;
using WebApiSample.Infrastructure;
public class EstimateDB
{
    public EstimateHeaderDB estHeaderDB {get; set;}
    public List<EstimateDetailDB> estDetailsDB {get; set;}
    public EstimateDB()
    {
        this.estDetailsDB=new List<EstimateDetailDB>();
        this.estHeaderDB=new EstimateHeaderDB();
    }
}