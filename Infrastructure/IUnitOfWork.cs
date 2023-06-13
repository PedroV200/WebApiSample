using WebApiSample.Infrastructure;

namespace WebApiSample.Infrastructure;

public interface IUnitOfWork
{
    IProductRepository  Products { get; }
    IIIBBrepository     IIBBs {get; }
    INCMrepository      NCMs {get; }
    IBancoRepository    Bancos {get; }
    IFwdtteRepository   Fwds {get;}
    ITerminalRepository Terminales {get; }
    IFleteRepository    Fletes {get; }
    ICustodiaRepository Custodias {get; }
    IGestdigitaldocRepository GestDigDoc {get; }
    IEmpresaRepository Empresas { get; }
    ICargaRepository Cargas { get; }

    ICanalRepository Canales {get; }
    IProveedorRepository Proveedores {get; }
    IDepositoRepository Depositos { get; }
    IPolizaRepository Polizas {get; }
    IImpuestoRepository Impuestos {get; }
    IEstimateHeaderDBRepository EstimateHeadersDB {get; }
    IEstimateDetailDBRepository EstimateDetailsDB {get; }
    ITarifasDepositoRepository TarifasDepositos {get;}
    ITarifasFwdContRepository TarifasFwdContenedores {get;}
    IContenedorRepository Contenedores {get;}
    ITarifasPolizaRepository TarifasPolizas {get;}
    ITarifasTteLocalRepository TarifasTtesLocal {get;}

    ICnstRepository Constantes {get;}
    //ITarifasSeguroRepository TarifasSeguros {get;}
    //ITarifasTerminalRepository TarifasTerminales {get;}
    //ITarifasTteRepository TarifasTransportes {get;}

    ITipoDeCambioRepository TiposDeCambio { get; }
    ISeguroRepository Seguros { get; }
    ITarifasTerminalRepository TarifasTerminals { get; }
    IUsuarioRepository Usuarios { get; }
    IDespachanteRepository Despachantes { get; }
}