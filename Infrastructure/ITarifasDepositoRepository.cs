using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasDepositoRepository:IGenericRepository<TarifasDeposito>
{
    Task<TarifasDeposito> GetByDepoContTypeAsync(string depo,string cont);
    Task<int> DeleteByDepoContTypeAsync(string depo,string cont);
    Task<int> UpdateByDepoContTypeAsync(TarifasDeposito entity);

}