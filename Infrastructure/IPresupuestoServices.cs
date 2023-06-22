using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IPresupuestoService : IGenericService<EstimateV2>
{
    public Task<EstimateV2>submitPresupuesto(EstimateDB miEst);
}