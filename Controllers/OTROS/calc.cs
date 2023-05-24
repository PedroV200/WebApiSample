using WebApiSample.Models;
using WebApiSample.Infrastructure;

public class calc
{
    // Con UnitOfWork accedo a todos los repositorios.
    private IUnitOfWork _unitOfWork;
    private IEstimateService _estService;

    public calc(IUnitOfWork unitOfWork, IEstimateService estService)
    {
        _unitOfWork=unitOfWork;
        _estService=estService;
    }


// Ejecuta el batch de calculos, de izq a derecha conforme el libro de XLS
// Segun "Presupuestador Argentina, libro N - Duchas Escocesas"

// Me dan como parametro el numero de presupuesto.
    public async Task<List<double>> calcBatch(int estNumber)
    {
        EstimateDB myEstDB=new EstimateDB(); 
        dbutils dbhelper=new dbutils(_unitOfWork);
        myEstDB=await dbhelper.getEstimateLastVers(estNumber);


        // El objeto Estimate que se definio. 
        EstimateV2 myEstV2=new EstimateV2();

        // Expando el EstimateDB a un EstimateV2
        myEstV2=dbhelper.transferDataFromDBType(myEstDB);

        // Hago algunas cuentas.
        // Calculo el peso total por articulo
        // COL J
        myEstV2=_estService.CalcPesoTotal(myEstV2);
        // COL K
        myEstV2=_estService.CalcCbmTotal(myEstV2);
        // COL L. Calculo el fob total por articulo
        myEstV2=_estService.CalcFobTotal(myEstV2);
        // CELDA L43. Sumo todos los fob totales. Sumatoria de L15-L41 que se copia en celda C3
        myEstV2.FobGrandTotal=_estService.sumFobTotal(myEstV2);
        // CELDA C4. Traigo la tarifa del flete desde BASE_TARIFAS por fowarder y tipo cont
        TarifasFwdCont myTar=await _unitOfWork.TarifasFwdContenedores.GetByFwdContTypeAsync(myEstV2.FreightFwd,myEstV2.FreightType);
        // De la consulta me quedo con el valor del flete (se usa 60%)
        myEstV2.FleteTotal=await _estService.lookUpTarifaFleteCont(myEstV2);
        // COL M. Calcula el flete ponderado a cada articulo del detalle.
        myEstV2=_estService.CalcFleteTotal(myEstV2);
        // COL N. Calcula el seguro ponderado a cada articulo del detalle 
        myEstV2=_estService.CalcSeguro(myEstV2);
        // CELDA AH. Calculo el factor de producto
        myEstV2=_estService.CalcFactorProdTotal(myEstV2);
        // COL O. Calcula el CIF que solo depende de los datos ya calculados previamente (COL L, N y M)
        //myEstV2=_estService.CalcCif(myEstV2);
        // COL R (COL O y COL Q no estan en uso)
        myEstV2=_estService.CalcAjusteIncDec(myEstV2);
        // COL S (die segun NCM)
        myEstV2=await _estService.searchNcmDie(myEstV2);
        // COL T
        myEstV2=_estService.CalcDerechos(myEstV2);
        // COL U
        myEstV2=await _estService.resolveNcmTe(myEstV2);
        // COL V
        myEstV2=_estService.CalcTasaEstad061(myEstV2);
        // COL X
        myEstV2=_estService.CalcBaseGcias(myEstV2); 
        // COL Y
        myEstV2=await _estService.searchIva(myEstV2);
        // COL Z
        myEstV2=_estService.CalcIVA415(myEstV2);
        // COL AA
        myEstV2= await _estService.searchIvaAdic(myEstV2);
        // COL AB
        myEstV2=_estService.CalcIVA_ad_Gcias(myEstV2);
        // COL AC
        myEstV2=_estService.CalcImpGcias424(myEstV2);
        // COL AD
        myEstV2=_estService.CalcIIBB900(myEstV2);
        // COL AE
        myEstV2=_estService.CalcPrecioUnitUSS(myEstV2);
        // COL AF
        myEstV2=_estService.CalcPagado(myEstV2);
        // Fin de las cuentas.

        // Tengo que devolver los pesos totales en el POST calculado como parte del ejemplo
        List<double> pesoTot=new List<double>();
        // Para cada porducto del detalle estraigo el peso Total:
        foreach(EstimateDetail ed in myEstV2.EstDetails)
        {
            pesoTot.Add(ed.PesoTot);
        }
// Devuelvo la lista de lo que saque.
        return pesoTot;
    }
}