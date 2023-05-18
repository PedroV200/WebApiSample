using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IContenedorRepository : IGenericRepository<Contenedor>
{
    public Task<Contenedor> GetByTipoContAsync(string type);
    public Task<int> DeleteByTipoContAsync(string type);

}