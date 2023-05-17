using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasFwdContRepository:IGenericRepository<TarifasFwdCont>
{
        Task<TarifasFwdCont> GetByFwdContTypeAsync(string fwd,string cont);
        Task<int> DeleteByFwdContTypeAsync(string fwd,string cont);
        Task<int> UpdateByFwdContTypeAsync(TarifasFwdCont entity);
}