using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class ProveedorController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ProveedorController(ILogger<ProductsController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Proveedores")]
    public async Task<IActionResult>Post(Proveedor entity)
    {
        try
        {
            var result=await _unitOfWork.Proveedores.AddAsync(entity);
            // Cero filas afectada ... we have problems.
            if(result==0)
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
    public async Task<IActionResult>Put(int id,Proveedor entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            if (id!=entity.id)
            {
                return BadRequest();
            }
            var result=await _unitOfWork.Proveedores.UpdateAsync(entity);
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
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult>Delete(int id)
    {
        try
        {
            var result=await _unitOfWork.Proveedores.DeleteAsync(id);
            // Ninguna fila afectada .... El id no existe
            if(result==0)
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
    public async Task<ActionResult<Proveedor>> Get(int id)
    {
        try
        {
            var result=await _unitOfWork.Proveedores.GetByIdAsync(id);
            if(result==null)
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

    [HttpGet(Name = "GetAll Proveedores")]
    public async Task<IEnumerable<Proveedor>> GetAll()
    {
        try
        {
            return await _unitOfWork.Proveedores.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
