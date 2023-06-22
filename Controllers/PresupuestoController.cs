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
    public async Task<EstimateV2>Post(EstimateDB entity)
    {
        return await _presupService.submitPresupuesto(entity);
    }

    
   [HttpGet("{id}/{vers}")]
    public async Task<ActionResult<EstimateDB>>Get(int id, int vers) 
    {
       
        EstimateDB tmpED=new EstimateDB();
  

        dbutils dbHelper=new dbutils(_unitOfWork);
        if(vers==0)
        {
            tmpED=await dbHelper.getEstimateLastVers(id);
        }
        else
        {
            tmpED=await dbHelper.GetEstimateByNumByVers(id,vers);
        }

        if(tmpED==null)
        {
            return NotFound();
        }
        else
        {
            return tmpED;
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
