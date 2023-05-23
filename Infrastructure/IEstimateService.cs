using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IEstimateService : IGenericService<EstimateV2>
{

    IEstimateDetailService estDetServices {get;}

    // CELDA I43
    //public EstimateV2 CalcCantPcsTotal(EstimateV2 est);
    // CELDA J43
    public EstimateV2 CalcPesoTotal(EstimateV2 est);
    // CELDA K43
    public EstimateV2 CalcCbmTotal(EstimateV2 est);
    // CELDA L43 y CELDA C3
    public EstimateV2 CalcFobTotal(EstimateV2 est);
    // CELDA M43
    public EstimateV2 CalcFleteTotal(EstimateV2 est);
    // COL N
    public EstimateV2 CalcSeguro(EstimateV2 est);
    // COL O
    
    // COL P y COL Q
    public EstimateV2 CalcAjusteIncDec(EstimateV2 est);

    // CELDA O43
    /*public double CalcCifTotal(Estimate est);
    // CELDA R43
    public double CalcValorAduanaDivisaTotal(Estimate est);
    // CELDA T43
    public double CalcDerechosTotal(Estimate est);
    // CELDA V43
    public double CalcTasaEstTotal(Estimate est);
    // CELDA X43
    public double CalcBaseIvaGciasTotal(Estimate est);
*/
    public EstimateV2 CalcFactorProdTotal(EstimateV2 est);
    // SIGUE ....

    public  Task<EstimateV2> searchNcmDie(EstimateV2 est);
     public Task<EstimateV2> resolveNcmTe(EstimateV2 est);
    // TOTALES DE COLUMNA .... FILA 43
    public double sumPesoTotal(EstimateV2 est);
    public double sumFobTotal(EstimateV2 est);

    // SIGUE .....
    }