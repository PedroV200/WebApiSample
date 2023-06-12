using WebApiSample.Infrastructure;
using WebApiSample.Core;
using WebApiSample.Models;

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
        services.AddTransient<IEstimateDetailDBRepository, EstimateDetailDBRepository>(); 
        services.AddTransient<IEstimateHeaderDBRepository, EstimateHeaderDBRepository>();
        services.AddTransient<ITarifasDepositoRepository, TarifasDepositoRepository>();
        services.AddTransient<ITarifasFwdContRepository, TarifasFwdContRepository>();
        services.AddTransient<IContenedorRepository, ContenedorRepository>();
        services.AddTransient<ITipoDeCambioRepository, TipoDeCambioRepository>();
        services.AddTransient<ISeguroRepository, SeguroRepository>();
        services.AddTransient<ITarifasTerminalRepository, TarifasTerminalRepository>();
        services.AddTransient<ITarifasPolizaRepository, TarifasPolizaRepository>();
        services.AddTransient<ITarifasTteLocalRepository, TarifasTteLocalRepository>();
        services.AddTransient<IEstimateService,EstimateService>();
        services.AddTransient<IEstimateDetailService, EstimateDetailService>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IUsuarioRepository, UsuarioRepository>();
    
    }
}