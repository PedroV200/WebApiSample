using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface ITipoDeCambioRepository : IGenericRepository<TipoDeCambio>
{
    public  Task<double> GetByDateAsync(string date);
}