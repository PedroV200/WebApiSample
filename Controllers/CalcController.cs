using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class CalcController : ControllerBase
{
    private readonly ILogger<CalcController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEstimateService _estService;

    private calc myCalc;

    public CalcController(ILogger<CalcController> logger, IUnitOfWork unitOfWork,IEstimateService estService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _estService = estService;
        //myEstimate = new Estimate(unitOfWork);
        myCalc = new calc(_unitOfWork,_estService);
    }

    [HttpPost(Name = "Post Estimate")]
    public async Task<List<double>>Post(EstimateDB entity)
    {
        var result=0;
        result=await _unitOfWork.EstimateHeadersDB.AddAsync(entity.estHeaderDB);
        foreach(EstimateDetailDB ed in entity.estDetailsDB)
        {
            result=await _unitOfWork.EstimateDetailsDB.AddAsync(ed);
        }
        return await myCalc.calcBatch(entity.estHeaderDB.Id);
    }

    
   [HttpGet("{id}")]
    public async Task<EstimateDB>Post(int id)
    {
        EstimateDB myEst=new EstimateDB(); 
        // La consulta de headers por un numero de presupuesto puede dar como resultado mas
        // de un header (multiples versiones del mismo presupuesto). 
        List<EstimateHeaderDB>misDetalles=new List<EstimateHeaderDB>();
        // La query se hace ORDENADA POR VERSION de MAYOR A MENOR. Es una LISTA de estHeaders
        var result=await _unitOfWork.EstimateHeadersDB.GetByEstNumberLastVersAsync(id);
        misDetalles=result.ToList();
        // El elemento 0 corresponde al header buscado en con la version MAYOR.
        myEst.estHeaderDB=misDetalles[0];
        // Con el PK del header anterior me voy a buscar los Details que lo referencia en su FK
        var result1=await _unitOfWork.EstimateDetailsDB.GetAllByIdEstHeadersync(myEst.estHeaderDB.Id);
        // De nuevo, la consulta x detalles, da una LISTA como resultado.
        myEst.estDetailsDB=result1.ToList();
        // Devuelvo el presupuesto.
        return myEst;
    }
   
}
