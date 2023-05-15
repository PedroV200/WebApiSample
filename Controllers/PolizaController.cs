using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class PolizaController : ControllerBase
{
    private readonly ILogger<PolizaController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public PolizaController(ILogger<PolizaController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Poliza")]
    public async Task<IActionResult>Post(Poliza entity)
    {
        var result=await _unitOfWork.Polizas.AddAsync(entity);
        // Cero filas afectada ... we have problems.
        if(result==0)
        {
            return NotFound();
        }
        // Ok
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>Put(int id,Poliza entity)
    {
        // Controlo que el id sea consistente.
        if(id!=entity.id)
        {
            return BadRequest();
        }
        var result=await _unitOfWork.Polizas.UpdateAsync(entity);
        // Si la operacion devolvio 0 filas .... es por que no le pegue al id.
        if(result==0)
        {
            return NotFound();
        }
        // Si llegue hasta aca ... OK
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult>Delete(int id)
    {
        var result=await _unitOfWork.Polizas.DeleteAsync(id);
        // Ninguna fila afectada .... El id no existe
        if(result==0)
        {
            return NotFound();
        }
        // Si llegue hasta aca, OK
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Poliza>> Get(int id)
    {
        var result=await _unitOfWork.Polizas.GetByIdAsync(id);
        if(result==null)
        {
            return NotFound();
        }
        else
        {
            return result;
        }
    }

    [HttpGet(Name = "GetAll Polizas")]
    public async Task<IEnumerable<Poliza>> GetAll()
    {
        return await _unitOfWork.Polizas.GetAllAsync();
    }
}
