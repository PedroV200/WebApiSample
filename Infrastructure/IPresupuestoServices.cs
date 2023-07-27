using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IPresupuestoService : IGenericService<EstimateV2>
{
    public Task<EstimateV2>submitPresupuestoUpdated(int id,EstimateDB miEst);
    public Task<EstimateV2>submitPresupuestoNew(EstimateDB miEst);
    public Task<EstimateV2>reclaimPresupuesto(int estNumber,int estVers);
    public Task<EstimateV2>acalcPresupuesto(EstimateDB miEst);
    public Task<EstimateV2>simulaPresupuesto(EstimateDB miEst);
    public string getLastErr();
}