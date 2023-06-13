using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITarifasPolizaRepository:IGenericRepository<TarifasPoliza>
{
    Task<TarifasPoliza> GetByDescAsync(string proveedor);
}