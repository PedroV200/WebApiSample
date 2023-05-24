using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UsuarioController(ILogger<ProductsController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Usuario")]
    public async Task<IActionResult> Post(Usuario entity)
    {
        try
        {
            var result = await _unitOfWork.Usuarios.AddAsync(entity);
            // Cero filas afectada ... we have problems.
            if (result == -1) return BadRequest("Error en el metodo AddAsync: No se pudo agregar el objeto Usuario.");
            if (result == 0) return NotFound();
            // Ok
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Usuario entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            if (id != entity.UserId)
            {
                return BadRequest();
            }
            var result = await _unitOfWork.Usuarios.UpdateAsync(entity);
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
            throw new Exception(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _unitOfWork.Usuarios.DeleteAsync(id);
            // Ninguna fila afectada .... El id no existe
            if (result == 0)
            {
                return NotFound();
            }
            // Si llegue hasta aca, OK
            return Ok(result);
        }
        catch (Exception)
        {
            throw new Exception($"Could not delete {id}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> Get(int id)
    {
        try
        {
            var result = await _unitOfWork.Usuarios.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return result;
            }
        }
        catch (Exception)
        {
            throw new Exception($"No existe Usuario con UserId {id}");
        }
    }

    [HttpGet(Name = "GetAll Usuarios")]
    public async Task<IEnumerable<Usuario>> GetAll()
    {
        try
        {
            return await _unitOfWork.Usuarios.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
