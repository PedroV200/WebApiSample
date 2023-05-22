using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class ContenedorController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ContenedorController(ILogger<ProductsController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Contenedor")]
    public async Task<IActionResult>Post(Contenedor entity)
    {
        try
        {
            var result=await _unitOfWork.Contenedores.AddAsync(entity);
            // Cero filas afectada ... we have problems.
            if(result==0)
            {
                return NotFound();
            }
            // Ok
            return Ok();
        }catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>Put(int id,Contenedor entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            //if(id!=entity.id)
            //{
                //return BadRequest();
            //}
            var result=await _unitOfWork.Contenedores.UpdateAsync(entity);
            // Si la operacion devolvio 0 filas .... es por que no le pegue al id.
            if(result==0)
            {
                return NotFound();
            }
            // Si llegue hasta aca ... OK
            return Ok(result);
        }catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{type}")]
    public async Task<IActionResult>Delete(string type)
    {
        try
        {

        var result=await _unitOfWork.Contenedores.DeleteByTipoContAsync(type);
        // Ninguna fila afectada .... El id no existe
        if(result==0)
        {
            return NotFound();
        }
        // Si llegue hasta aca, OK
        return Ok(result);
        }catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{type}")]
    public async Task<ActionResult<Contenedor>> Get(string type)
    {
        try
        {
            var result=await _unitOfWork.Contenedores.GetByTipoContAsync(type);
            if(result==null)
            {
                return NotFound();
            }
            else
            {
                return result;
            }
        }catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet(Name = "GetAll Contenedor")]
    public async Task<IEnumerable<Contenedor>> GetAll()
    {
        try
        {
            return await _unitOfWork.Contenedores.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
