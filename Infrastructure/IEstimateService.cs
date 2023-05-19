using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IEstimateService : IGenericService<EstimateV2>
{
    // CELDA I43
    //public EstimateV2 CalcCantPcsTotal(EstimateV2 est);
    // CELDA J43
    public EstimateV2 CalcPesoTotal(EstimateV2 est);
    // CELDA K43
    //public EstimateV2 CalcCbmTotal(EstimateV2 est);
    // CELDA L43 y CELDA C3
    public EstimateV2 CalcFobTotal(EstimateV2 est);
    /*// CELDA M43
    public double CalcFleteTotal(Estimate est);
    // CELDA N43
    public double CalcSeguroTotal(Estimate est);
    // CELDA O43
    public double CalcCifTotal(Estimate est);
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


    // TOTALES DE COLUMNA .... FILA 43
    public double sumPesoTotal(EstimateV2 est);
    public double sumFobTotal(EstimateV2 est);

    // SIGUE .....
    }