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
    //IEstimateHeaderRepositoy EstimateHeaders {get; }
    IEstimateDetailRepository EstimateDetails {get; }
}