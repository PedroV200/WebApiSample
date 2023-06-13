using WebApiSample.Models;

namespace WebApiSample.Infrastructure;



public interface ICnstService : IGenericService<Cnst>
{

    //IEstimateDetailService estDetServ{get;}

    // COL J
    public Task<CONSTANTES> getConstantes();
}