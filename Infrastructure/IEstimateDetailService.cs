using WebApiSample.Models;

namespace WebApiSample.Infrastructure;

public interface IEstimateDetailService : IGenericService<EstimateDetail>
{

    //IEstimateDetailService estDetServ{get;}

    // COL J
    public double CalcPesoTotal(EstimateDetail estD);
    // COL K
   //public double CalcCBMTotal(EstimateDetail estD);
    // COL L
    public double CalcFob(EstimateDetail estD);
    // COL M
  /*  public double CalcFlete(EstimateDetail estD);
    // COL N
    public double CalcSeguro(EstimateDetail estD);
    // COL O
    public double CalcCIF(EstimateDetail estD);
    // COL R
    public double CalcAduanaDivisa(EstimateDetail estD);
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

}