using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IEstimateDetailDBRepository : IGenericRepository<EstimateDetailDB>
{
     Task<IEnumerable<EstimateDetailDB>>GetAllByCodeAsync(int code);
}