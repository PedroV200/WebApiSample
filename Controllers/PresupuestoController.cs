using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

// LISTED 6/7/2023 11:16 AM

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

// Este endpoint es para un presupuesto nuevo. Notar que no se pasa el id
    [HttpPost(Name = "Post Presupuesto Nuevo")]
    public async Task<ActionResult<EstimateV2>>PostNewPresup(EstimateDB entity)
    {
        var result=await _presupService.submitPresupuestoNew(entity);
        if(result==null)
        {
            return BadRequest(_presupService.getLastErr());
        }
        else
        {
            return result;
        }
    }

// Este endpoint es para un presupuesto nuevo. Notar que no se pasa el id
    [HttpPost("{id}")]
    public async Task<ActionResult<EstimateV2>>PostUpdatedPresup(int id,EstimateDB entity)
    {
        var result=await _presupService.submitPresupuestoUpdated(id, entity);
        if(result==null)
        {
            return BadRequest(_presupService.getLastErr());
        }
        else
        {
            return result;
        }
    }

// Este endpoint es para un presupuesto nuevo. Notar que no se pasa el id
    [HttpPost("/acalc")]
    public async Task<ActionResult<EstimateV2>>PostAcalcPresupuesto(EstimateDB entity)
    {
        var result=await _presupService.acalcPresupuesto(entity);
        if(result==null)
        {
            return BadRequest(_presupService.getLastErr());
        }
        else
        {
            return result;
        }
    }

    [HttpPost("/sim")]
    public async Task<ActionResult<EstimateV2>>PostSimulPresupuesto(EstimateDB entity)
    {
        var result=await _presupService.simulaPresupuesto(entity);
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
    // 6/7/2023 Se agrega este endpoint para listar todas las versiones de un presupuesto.
    [HttpGet("{id}")]
    public async Task<ActionResult<List<EstimateHeaderDB>>>GetVersionesFromEstimate(int id) 
    {
        var result=await _unitOfWork.EstimateHeadersDB.GetAllVersionsFromEstimate(id);
        return result.ToList();
        
    }

    [HttpDelete("{estNumber}/{estVers}")]
    public async Task<int>Delete(int estNumber,int estVers)
    {
        dbutils dbHelper=new dbutils(_unitOfWork);
        return await dbHelper.deleteEstimateByNumByVers(estNumber,estVers);
    }
   
}
