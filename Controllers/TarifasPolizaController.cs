using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class TarifasPolizaController : ControllerBase
{
    private readonly ILogger<TarifasPolizaController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TarifasPolizaController(ILogger<TarifasPolizaController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post TarfiasPoliza")]
    public async Task<IActionResult>Post(TarifasPoliza entity)
    {
        try
        {
            var result=await _unitOfWork.TarifasPolizas.AddAsync(entity);
            // Cero filas afectada ... we have problems.
            if(result==0)
            {
                return NotFound();
            }
            // Ok
            return Ok();
        }catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>Put(int id,TarifasPoliza entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            if (id!=entity.id)
            {
                return BadRequest();
            }
            var result=await _unitOfWork.TarifasPolizas.UpdateAsync(entity);
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
            var result=await _unitOfWork.TarifasPolizas.DeleteAsync(id);
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
    public async Task<ActionResult<TarifasPoliza>> Get(int id)
    {
        try
        {
            var result=await _unitOfWork.TarifasPolizas.GetByIdAsync(id);
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

    [HttpGet(Name = "GetAll Tarifas Polizas")]
    public async Task<IEnumerable<TarifasPoliza>> GetAll()
    {
        try
        {
            return await _unitOfWork.TarifasPolizas.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
