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
    public async Task<IActionResult>Post(EstimateHeader entity)
    {
        var result=await _unitOfWork.EstimateHeaders.AddAsync(entity);
        // Cero filas afectada ... we have problems.
        if(result==0)
        {
            return NotFound();
        }
        // Ok
        return Ok();
    }

    [HttpPut("{code}")]
    public async Task<IActionResult>Put(int code,EstimateHeader entity)
    {
        // Controlo que el id sea consistente.
        if(code!=entity.code)
        {
            return BadRequest();
        }
        var result=await _unitOfWork.EstimateHeaders.UpdateAsync(entity);
        // Si la operacion devolvio 0 filas .... es por que no le pegue al id.
        if(result==0)
        {
            return NotFound();
        }
        // Si llegue hasta aca ... OK
        return Ok(result);
    }

    [HttpDelete("{code}")]
    public async Task<IActionResult>Delete(int code)
    {
        var result=await _unitOfWork.EstimateHeaders.DeleteAsync(code);
        // Ninguna fila afectada .... El id no existe
        if(result==0)
        {
            return NotFound();
        }
        // Si llegue hasta aca, OK
        return Ok(result);
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<EstimateHeader>> Get(int code)
    {
        var result=await _unitOfWork.EstimateHeaders.GetByIdAsync(code);
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
    public async Task<IEnumerable<EstimateHeader>> GetAll()
    {
        return await _unitOfWork.EstimateHeaders.GetAllAsync();
    }
}
