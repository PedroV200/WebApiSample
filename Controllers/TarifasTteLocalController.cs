using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

// LISTED 12/06/2023 12:27PM

[ApiController]
[Route("[controller]")]
public class TarifasTteLocalController : ControllerBase
{
    private readonly ILogger<TarifasTteLocalController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TarifasTteLocalController(ILogger<TarifasTteLocalController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Tte Local")]
    public async Task<IActionResult>Post(TarifasTteLocal entity)
    {
        try
        {
            var result=await _unitOfWork.TarifasTtesLocal.AddAsync(entity);
            // Cero filas afectada ... we have problems.
            if(result==0)
            {
                return NotFound();
            }
            // Ok
            return Ok();
        }catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>Put(int id,TarifasTteLocal entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            if(id!=entity.id)
            {
                return BadRequest();
            }
            var result=await _unitOfWork.TarifasTtesLocal.UpdateAsync(entity);
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
            return BadRequest(ex);
        }
    }

    //[HttpDelete("{id}")]
    [HttpDelete("{id}")]
    public async Task<IActionResult>Delete(int id)
    {
        try
        {
            var result=await _unitOfWork.TarifasTtesLocal.DeleteAsync(id);
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
            return BadRequest(ex);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TarifasTteLocal>> Get(string contype)
    {
        try
        {
            var result=await _unitOfWork.TarifasTtesLocal.GetTarifaTteByContAsync(contype);
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
            return BadRequest(ex);
        }
    }

    [HttpGet(Name = "GetAll Tarifas TteLocal")]
    public async Task<IEnumerable<TarifasTteLocal>> GetAll()
    {
        try
        {
            return await _unitOfWork.TarifasTtesLocal.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}