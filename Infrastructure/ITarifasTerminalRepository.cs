using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasTerminalRepository : IGenericRepository<TarifasTerminal>
{
    Task<TarifasTerminal> GetByDepoContTypeAsync(string depo, string cont);
    Task<int> DeleteByDepoContTypeAsync(string depo, string cont);
    Task<int> UpdateByDepoContTypeAsync(TarifasTerminal entity);

}