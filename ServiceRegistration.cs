using WebApiSample.Infrastructure;
using WebApiSample.Core;

public static class ServiceRegistration
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<IIIBBrepository, IIBBRepository>();
        services.AddTransient<INCMrepository, NCMrepository>();
        services.AddTransient<IBancoRepository, BancoRepository>();
        services.AddTransient<IFwdtteRepository, FwdtteRepository>();
        services.AddTransient<ITerminalRepository, TerminalRepository>();
        services.AddTransient<IFleteRepository, FleteRepository>();
        services.AddTransient<ICustodiaRepository, CustodiaRepository>();
        services.AddTransient<IGestdigitaldocRepository, GestdigitaldocRepository>();
        services.AddTransient<IEmpresaRepository, EmpresaRepository>();
        services.AddTransient<ICargaRepository, CargaRepository>();
        services.AddTransient<ICanalRepository, CanalRepository>();
        services.AddTransient<IProveedorRepository, ProveedorRepository>();
        services.AddTransient<IDepositoRepository, DepositoRepository>();
        services.AddTransient<IPolizaRepository, PolizaRepository>();
        services.AddTransient<IImpuestoRepository, ImpuestoRepository>();
        //services.AddTransient<IProductRepository, ProductRepositoryMemory>();
        services.AddTransient<IEstimateDetailRepository, EstimateDetailRepository>(); 
        services.AddTransient<IEstimateHeaderRepository, EstimateHeaderRepository>();
//<<<<<<< pedro
        services.AddTransient<ITarifasDepositoRepository, TarifasDepositoRepository>();
        services.AddTransient<ITarifasFwdContRepository, TarifasFwdContRepository>();
//=======
        services.AddTransient<ITipoDeCambioRepository, TipoDeCambioRepository>();
//>>>>>>> main
        services.AddTransient<IUnitOfWork, UnitOfWork>();
    }
}