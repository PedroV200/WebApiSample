using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;



[ApiController]
[Route("[controller]")]
public class PresupuestoController : ControllerBase
{
    private readonly ILogger<PresupuestoController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEstimateService _estService;
    private readonly IPresupuestoService _presupService;


    private calc myCalc;

    public PresupuestoController(ILogger<PresupuestoController> logger, IUnitOfWork unitOfWork,IEstimateService estService, IPresupuestoService presupService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _estService = estService;
        _presupService=presupService;
    }

    [HttpPost(Name = "Post Estimate")]
    public async Task<ActionResult<EstimateV2>>Post(EstimateDB entity)
    {
        var result=await _presupService.submitPresupuesto(entity);
        if(result==null)
        {
            return BadRequest(_presupService.getLastErr());
        }
        else
        {
            return result;
        }
    }

    
   [HttpGet("{id}/{vers}")]
    public async Task<ActionResult<EstimateV2>>Get(int id, int vers) 
    {
       
        EstimateV2 myEst=new EstimateV2();

        myEst= await _presupService.reclaimPresupuesto(id,vers);

        if(myEst==null)
        {
            return NotFound();
        }
        else
        {
            return myEst;
        }
    }


   [HttpGet]
    public async Task<ActionResult<List<EstimateHeaderDB>>>GetHeaders() 
    {
        var result=await _unitOfWork.EstimateHeadersDB.GetAllAsync();
        return result.ToList();
        
    }

    [HttpDelete("{estNumber}/{estVers}")]
    public async Task<int>Delete(int estNumber,int estVers)
    {
        dbutils dbHelper=new dbutils(_unitOfWork);
        return await dbHelper.deleteEstimateByNumByVers(estNumber,estVers);
    }
   
}
