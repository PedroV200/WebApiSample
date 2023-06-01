using WebApiSample.Models;

namespace WebApiSample.Infrastructure;
public interface IIIBBrepository : IGenericRepository<IIBB>
{
    public Task<double> GetSumFactores();
}