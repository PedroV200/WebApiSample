using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class FleteController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public FleteController(ILogger<ProductsController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Flete")]
    public async Task<IActionResult>Post(Flete entity)
    {
        var result=await _unitOfWork.Fletes.AddAsync(entity);
        // Cero filas afectada ... we have problems.
        if(result==0)
        {
            return NotFound();
        }
        // Ok
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>Put(int id,Flete entity)
    {
        // Controlo que el id sea consistente.
        if(id!=entity.id)
        {
            return BadRequest();
        }
        var result=await _unitOfWork.Fletes.UpdateAsync(entity);
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
        var result=await _unitOfWork.Fletes.DeleteAsync(id);
        // Ninguna fila afectada .... El id no existe
        if(result==0)
        {
            return NotFound();
        }
        // Si llegue hasta aca, OK
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Flete>> Get(int id)
    {
        var result=await _unitOfWork.Fletes.GetByIdAsync(id);
        if(result==null)
        {
            return NotFound();
        }
        else
        {
            return result;
        }
    }

    [HttpGet(Name = "GetAll Flete")]
    public async Task<IEnumerable<Flete>> GetAll()
    {
        return await _unitOfWork.Fletes.GetAllAsync();
    }
}
