using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class EstimateHeaderController : ControllerBase
{
    private readonly ILogger<EstimateHeaderController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public EstimateHeaderController(ILogger<EstimateHeaderController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Estimate Header")]
    public async Task<IActionResult>Post(EstimateHeaderDB entity)
    {
        var result=await _unitOfWork.EstimateHeadersDB.AddAsync(entity);
        // Cero filas afectada ... we have problems.
        if(result==0)
        {
            return NotFound();
        }
        // Ok
        return Ok();
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult>Put(int Id,EstimateHeaderDB entity)
    {
        // Controlo que el id sea consistente.
        if(Id!=entity.Id)
        {
            return BadRequest();
        }
        var result=await _unitOfWork.EstimateHeadersDB.UpdateAsync(entity);
        // Si la operacion devolvio 0 filas .... es por que no le pegue al id.
        if(result==0)
        {
            return NotFound();
        }
        // Si llegue hasta aca ... OK
        return Ok(result);
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult>Delete(int Id)
    {
        var result=await _unitOfWork.EstimateHeadersDB.DeleteAsync(Id);
        // Ninguna fila afectada .... El id no existe
        if(result==0)
        {
            return NotFound();
        }
        // Si llegue hasta aca, OK
        return Ok(result);
    }

    [HttpGet("{Id}")]
    public async Task<ActionResult<EstimateHeaderDB>> Get(int Id)
    {
        var result=await _unitOfWork.EstimateHeadersDB.GetByIdAsync(Id);
        if(result==null)
        {
            return NotFound();
        }
        else
        {
            return result;
        }
    }

    [HttpGet(Name = "GetAll Estimate Header")]
    public async Task<IEnumerable<EstimateHeaderDB>> GetAll()
    {
        return await _unitOfWork.EstimateHeadersDB.GetAllAsync();
    }
}
