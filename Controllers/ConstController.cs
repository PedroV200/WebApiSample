using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

// LISTED 13_06_2023 13:11

[ApiController]
[Route("[controller]")]
public class ConstController : ControllerBase
{
    private readonly ILogger<ConstController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ConstController(ILogger<ConstController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Constante")]
    public async Task<IActionResult>Post(Cnst entity)
    {
        try
        {
            var result=await _unitOfWork.Constantes.AddAsync(entity);
            // Cero filas afectada ... we have problems.
            if(result == -1) return BadRequest("Error en el metodo AddAsync: No se pudo agregar el objeto Banco.");
            if(result == 0) return NotFound();
            // Ok
            return Ok();
        }catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>Put(int id,Cnst entity)
    {
        try {
            // Controlo que el id sea consistente.
            if(id!=entity.id)
            {
                return BadRequest();
            }
            var result=await _unitOfWork.Constantes.UpdateAsync(entity);
            // Si la operacion devolvio 0 filas .... es por que no le pegue al id.
            if(result==0)
            {
                return NotFound();
            }
            // Si llegue hasta aca ... OK
            return Ok(result);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult>Delete(int id)
    {
        try { 
            var result=await _unitOfWork.Constantes.DeleteAsync(id);
            // Ninguna fila afectada .... El id no existe
            if(result==0)
            {
                return NotFound();
            }
            // Si llegue hasta aca, OK
            return Ok(result);
        }catch (Exception)
        {
            throw new Exception($"Could not delete {id}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cnst>> Get(int id)
    {
        try
        {
            var result=await _unitOfWork.Constantes.GetByIdAsync(id);
            if(result==null)
            {
                return NotFound();
            }
            else
            {
                return result;
            }
        }catch (Exception)
        {
            throw new Exception($"No existe Banco con Id {id}");
        }
    }

    [HttpGet(Name = "GetAll Constantes")]
    public async Task<IEnumerable<Cnst>> GetAll()
    {
        try
        {
            return await _unitOfWork.Constantes.GetAllAsync();
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
