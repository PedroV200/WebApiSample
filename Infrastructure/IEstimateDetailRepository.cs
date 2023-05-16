using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IEstimateDetailRepository : IGenericRepository<EstimateDetail>
{
     Task<IEnumerable<EstimateDetail>>GetAllByCodeAsync(int code);
}