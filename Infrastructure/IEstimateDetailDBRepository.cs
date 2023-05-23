using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IEstimateDetailDBRepository : IGenericRepository<EstimateDetailDB>
{
     Task<IEnumerable<EstimateDetailDB>>GetAllByIdEstHeadersync(int code);
      public Task<int> DeleteByIdEstHeaderAsync(int IdEstHeader);
     
}