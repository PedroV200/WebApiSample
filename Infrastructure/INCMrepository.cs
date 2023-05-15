using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface INCMrepository : IGenericRepository<NCM>
{
    Task<NCM> GetByIdStrAsync(string id);
    Task<int> DeleteByStrAsync(string id);
}