using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IEstimateHeaderDBRepository : IGenericRepository<EstimateHeaderDB>
{
    
    public Task<IEnumerable<EstimateHeaderDB>> GetByEstNumberLastVersAsync(int estNumber);
}