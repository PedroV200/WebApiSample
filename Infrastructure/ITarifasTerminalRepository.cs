using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasTerminalRepository : IGenericRepository<TarifasTerminal>
{
    Task<TarifasTerminal> GetByContTypeAsync(string cont);
    Task<int> DeleteByContTypeAsync(string cont);
    Task<int> UpdateByDepoContTypeAsync(TarifasTerminal entity);
}