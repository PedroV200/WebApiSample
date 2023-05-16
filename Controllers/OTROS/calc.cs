using WebApiSample.Models;
using WebApiSample.Infrastructure;

public class calc
{
    private IUnitOfWork _unitOfWork;

    public calc(IUnitOfWork unitOfWork)
    {
        _unitOfWork=unitOfWork;
    }

    // Hace un query a la la lista de detalles del estimate.
    // que son todos los productos que conforman el estimate
    public async Task<List<EstimateDetail>> getItems()
    {
        var result= await _unitOfWork.EstimateDetails.GetAllAsync();
        return result.Cast<EstimateDetail>().ToList();

    }

    // Pide la lista de productos del estimate y computa para cada uno de ellos
    // el CBM usando los datos que acompañan a cada producto.
    public async Task<List<double>> calculateCBM()
    {
        List<double> cbmTot=new List<double>();
        List<EstimateDetail> estDetails=await getItems();
        foreach(EstimateDetail ed in estDetails)
        {
            double tmp=(ed.cantpcs*ed.cbmxcaja)/ed.pcsxcaja; 
            cbmTot.Add(tmp);   
        }
        return cbmTot;
    }
}