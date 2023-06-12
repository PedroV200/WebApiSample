using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasTteLocalRepository:IGenericRepository<TarifasTteLocal>
{
    public  Task<TarifasTteLocal> GetByTteTarifaByContAsync(string cont);
    public Task<int> UpdateTteTarifaByContTypeAsync(TarifasTteLocal entity);
}