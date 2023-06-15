using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasTteLocalRepository:IGenericRepository<TarifasTteLocal>
{
    public  Task<TarifasTteLocal> GetTarifaTteByContAsync(string cont);
    public Task<int> UpdateTteTarifaByContTypeAsync(TarifasTteLocal entity);
}