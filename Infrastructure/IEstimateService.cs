using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IEstimateService : IGenericService<EstimateV2>
{

    IEstimateDetailService _estDetServices {get;}

    public Task<EstimateV2> loadConstants(EstimateV2 est);

    // CELDA I43
    //public EstimateV2 CalcCantPcsTotal(EstimateV2 est);
    // CELDA J43
    public EstimateV2 CalcPesoTotal(EstimateV2 est);
    // CELDA K43
    public EstimateV2 CalcCbmTotal(EstimateV2 est);
    // CELDA L43 y CELDA C3
    public EstimateV2 CalcFobTotal(EstimateV2 est);
    // CELDA M43
    public EstimateV2 CalcFleteTotalByProd(EstimateV2 est);
    // COL N
    public EstimateV2 CalcSeguro(EstimateV2 est);
    // COL O
     public EstimateV2 CalcCif(EstimateV2 est);
    // COL P y COL Q -> COL R
    public EstimateV2 CalcAjusteIncDec(EstimateV2 est);
    // COL S
    public  Task<EstimateV2> searchNcmDie(EstimateV2 est);
    // COL T
    public EstimateV2 CalcDerechos(EstimateV2 est);
    // COL U
    public Task<EstimateV2> resolveNcmTe(EstimateV2 est);
    // COL V
    public EstimateV2 CalcTasaEstad061(EstimateV2 est);
    // COL X
    public EstimateV2 CalcBaseGcias(EstimateV2 est);
    // COL Y
    public Task<EstimateV2> searchIva(EstimateV2 est);
    // COL Z
    public EstimateV2 CalcIVA415(EstimateV2 est);
    // COL AA
    public Task<EstimateV2> searchIvaAdic(EstimateV2 est);
    // COL AB
    public EstimateV2 CalcIVA_ad_Gcias(EstimateV2 est);
    // COL AC
    public EstimateV2 CalcImpGcias424(EstimateV2 est);
    // COL AD
    public EstimateV2 CalcIIBB900(EstimateV2 est);
    // COL AE
    public EstimateV2 CalcPrecioUnitUSS(EstimateV2 est);
    // COL AF
    public EstimateV2 CalcPagado(EstimateV2 est);

    public EstimateV2 CalcFactorProdTotal(EstimateV2 est);

    public EstimateV2 CalcExtraGastoLocProyecto(EstimateV2 est);

    public EstimateV2 CalcExtraGastoProyectoUSS(EstimateV2 est);

    public EstimateV2 CalcExtraGastoProyectoUnitUSS(EstimateV2 est);

    public EstimateV2 CalcOverhead(EstimateV2 est);

    public EstimateV2 CalcCostoUnitarioUSS(EstimateV2 est);

    public EstimateV2 CalcCostoUnitario(EstimateV2 est); 

    public Task<double> calcularGastosProyecto(EstimateV2 miEst);

    public Task<double> lookUpTarifaFleteCont(EstimateV2 est);

    public Task<EstimateV2> CalcularCantContenedores(EstimateV2 est);

    public EstimateV2 CalcCbmGrandTotal(EstimateV2 est);

    public EstimateV2 CalcCifTotal(EstimateV2 est);

    public Task<EstimateV2> CalcFleteTotal(EstimateV2 est);

    public double sumPesoTotal(EstimateV2 est);

    public double sumFobTotal(EstimateV2 est);

    public EstimateV2 CalcSeguroTotal(EstimateV2 miEst);

    public EstimateV2 CalcPagadoTot(EstimateV2 miEst);

    public Task<EstimateV2> search_NCM_DATA(EstimateV2 est);

    public string getLastError();

    }