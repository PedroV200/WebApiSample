using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IEstimateDetailService : IGenericService<EstimateDetail>
{
    public void loadConstants(CONSTANTES miaConst);
    //IEstimateDetailService estDetServ{get;}

    // COL J
    public double CalcPesoTotal(EstimateDetail estD);
    // COL K
    public double CalcCbmTotal(EstimateDetail estD);
    // COL L
    public double CalcFob(EstimateDetail estD);
    // COL M
    public double CalcFlete(EstimateDetail estD, double fleteTotal, double fobGrandTotal);
    // COL N
    public double CalcSeguro(EstimateDetail estD, double seguroTotal, double fobGrandTotal);
    // COL O
    public double CalcCif(EstimateDetail est);
    // COL (P y Q)->R
    public double CalcValorEnAduanaDivisa(EstimateDetail estD);
    // COL T
    public Task<double> lookUpDie(EstimateDetail estDetails);
    // COL S
    public double CalcDerechos(EstimateDetail est);
    // COL U
    public Task<double> lookUpTe(EstimateDetail estDetails);
    // COL V
    public double CalcTasaEstad061(EstimateDetail estDetails);
    // COL X
    public double CalcBaseIvaGcias(EstimateDetail estDetails);
    // COL Y
    public Task<double> lookUpIVA(EstimateDetail estDetails);
    // COL Z
    public double CalcIVA415(EstimateDetail estDetail);
    // COL AA
    public Task<double> lookUpIVAadic(EstimateDetail estDetails);
    // COL AB
    public double CalcIvaAdic(EstimateDetail estDetails, bool ivaEx);
    // COL AC
    public double CalcImpGcias424(EstimateDetail estDetails);
    // COL AD
    public Task<double> CalcIIBB(EstimateDetail estDetails);
    // COL AE
    public double CalcPrecioUnitUSS(EstimateDetail estDetails);
    // COL AF
    public double CalcPagado(EstimateDetail estDetails);
    // TOTALES DE COLUMNA .... FILA 43
    //public double CalcCIF(EstimateDetail estD);
    // COL R
  /*  public double CalcAduanaDivisa(EstimateDetail estD);
    // COL S. Determina el decrecho de importacion dada la pos aranc (via QUERY a la Tabla NCM)
    public double LookUpDie(EstimateDetail estD);
    // COL T    
    public double CalcDerechos(EstimateDetail estD);
    // COL U. Determina la Tasa Estadistica dada la pos aranc (via QUERY a la Tabla NCM)
    public double LookUpTe(EstimateDetail estD);
    // COL V
    public double CalcTasaEstadistica(EstimateDetail estD);
    // COL X
    public double CalcBaseIvaGcias(EstimateDetail estD);
    // COL Y. Determina el IVA dada la pos aranc (via QUERY a la tabla NCM)
    public double LookUpIVA(EstimateDetail estD);

    // SEGUIR HASTA LA COL AN*/
    public double CalcFactorProducto(EstimateDetail estD, double fobTotal);

    public double CalcGastosProyPond(EstimateDetail estD, double gastosProy);

    public double CalcGastosProyPondUSS(EstimateDetail estD,double dolar);
    
    public double CalcGastosProyPorUnidUSS(EstimateDetail estD);

    public double CalcOverHeadUnitUSS(EstimateDetail estD);

    public double CalcCostoUnitUSS(EstimateDetail estD);

    public double CalcCostoUnit(EstimateDetail estD, double dolar);


}