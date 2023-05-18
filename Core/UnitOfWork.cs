namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(IProductRepository productRepository, 
                      IIIBBrepository IIBBrepository,
                      INCMrepository NCMrepository,
                      IBancoRepository BancoRepository,
                      IFwdtteRepository FwdtteRepository,
                      ITerminalRepository TermimalRepository,
                      IFleteRepository FleteRepository,
                      ICustodiaRepository CustodiaRepository,
                      IGestdigitaldocRepository GestDigDocRepository,
                      IEmpresaRepository EmpresaRepository,
                      ICargaRepository CargaRepository,
                      IDepositoRepository DepositoRepository,
                      IPolizaRepository PolizaRepository,
                      IImpuestoRepository ImpuestoRepository,
                      ICanalRepository CanalRepository,
                      IProveedorRepository ProveedorRepository,
                      IEstimateHeaderRepository EstimateHeaderRepository,
                      IEstimateDetailRepository EstimateDetailRepository,
                      ITarifasDepositoRepository tarifasDepositoRepository,
                      ITarifasFwdContRepository tarifasFwdContRepository,
                      IContenedorRepository ContenedorRepository,
                      //ITarifasSeguroRepository tarifasSeguroRepository,
                      //ITarifasTerminalRepository tarifasTerminalRepository,
                      //ITarifasTteRepository tarifasTteRepository,
                      ITipoDeCambioRepository tipoDeCambioRepository,
                      ISeguroRepository seguroRepository,
                      ITarifasTerminalRepository TarifasTerminalRepository
                        
                      )
    {
        Products = productRepository;
        IIBBs = IIBBrepository;
        NCMs = NCMrepository;
        Bancos = BancoRepository;
        Fwds = FwdtteRepository;
        Terminales = TermimalRepository;
        Fletes = FleteRepository;
        Custodias = CustodiaRepository;
        GestDigDoc = GestDigDocRepository;
        Empresas = EmpresaRepository;
        Cargas = CargaRepository;
        Depositos = DepositoRepository;
        Polizas = PolizaRepository;
        Impuestos = ImpuestoRepository;
        Canales = CanalRepository;
        Proveedores = ProveedorRepository;
        EstimateHeaders = EstimateHeaderRepository;
        EstimateDetails = EstimateDetailRepository;        
        TarifasDepositos = tarifasDepositoRepository;
        TarifasFwdContenedores = tarifasFwdContRepository;
        Contenedores = ContenedorRepository;
        //TarifasSeguros = tarifasSeguroRepository;
        //TarifasTerminales = tarifasTerminalRepository;
        //TarifasTransportes = tarifasTteRepository;

        EstimateDetails = EstimateDetailRepository;
        TiposDeCambio = tipoDeCambioRepository;
        Seguros = seguroRepository;
        TarifasTerminals = TarifasTerminalRepository;

    }

    public IProductRepository Products { get; } 
    public IIIBBrepository IIBBs { get; } 
    public INCMrepository NCMs { get;}
    public IBancoRepository Bancos {get;}
    public IFwdtteRepository Fwds {get;}
    public ITerminalRepository Terminales {get; }
    public IFleteRepository Fletes {get; }
    public ICustodiaRepository Custodias {get; }
    public IGestdigitaldocRepository GestDigDoc {get; }
    public IEmpresaRepository Empresas { get; }
    public ICargaRepository Cargas { get; }
    public IDepositoRepository Depositos { get; }
    public IPolizaRepository Polizas { get; }
    public IImpuestoRepository Impuestos { get; }
    public IEstimateHeaderRepository EstimateHeaders { get;}
    public IEstimateDetailRepository EstimateDetails { get;}
    public ICanalRepository Canales {get;}
    public IProveedorRepository Proveedores {get;}

    public ITarifasDepositoRepository TarifasDepositos {get;}
    public ITarifasFwdContRepository TarifasFwdContenedores {get;}
    public IContenedorRepository Contenedores {get;}
    //public ITarifasSeguroRepository TarifasSeguros {get;}
    //public ITarifasTerminalRepository TarifasTerminales{get;}
    //public ITarifasTteRepository TarifasTransportes {get;}
    public ITipoDeCambioRepository TiposDeCambio { get; }
    public ISeguroRepository Seguros { get; }
    public ITarifasTerminalRepository TarifasTerminals { get;}

}