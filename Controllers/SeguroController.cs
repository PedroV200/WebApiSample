using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class SeguroController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public SeguroController(ILogger<ProductsController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Seguro")]
    public async Task<IActionResult> Post(Seguro entity)
    {
        try
        {
            var result = await _unitOfWork.Seguros.AddAsync(entity);
            // Cero filas afectada ... we have problems.
            if (result == 0)
            {
                return NotFound();
            }
            // Ok
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Seguro entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            if (id != entity.id)
            {
                return BadRequest();
            }
            var result = await _unitOfWork.Seguros.UpdateAsync(entity);
            // Si la operacion devolvio 0 filas .... es por que no le pegue al id.
            if (result == 0)
            {
                return NotFound();
            }
            // Si llegue hasta aca ... OK
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _unitOfWork.Seguros.DeleteAsync(id);
            // Ninguna fila afectada .... El id no existe
            if (result == 0)
            {
                return NotFound();
            }
            // Si llegue hasta aca, OK
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Seguro>> Get(int id)
    {
        try
        {
            var result = await _unitOfWork.Seguros.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return result;
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet(Name = "GetAll Seguro")]
    public async Task<IEnumerable<Seguro>> GetAll()
    {
        try
        {
            return await _unitOfWork.Seguros.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
