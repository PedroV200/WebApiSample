using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IEstimateHeaderDBRepository : IGenericRepository<EstimateHeaderDB>
{
    
    public Task<IEnumerable<EstimateHeaderDB>> GetByEstNumberLastVersAsync(int estNumber);
    public Task<EstimateHeaderDB> GetByEstNumberAnyVersAsync(int estnumber, int estVers);
}