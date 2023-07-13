using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

// LISTED 12/7/2023 15:26PM

[ApiController]
[Route("[controller]")]
public class TarifasTerminalController : ControllerBase
{
    private readonly ILogger<TarifasTerminalController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TarifasTerminalController(ILogger<TarifasTerminalController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Terminal/tarifa")]
    public async Task<IActionResult> Post(TarifasTerminal entity)
    {
        try
        {
            var result = await _unitOfWork.TarifasTerminals.AddAsync(entity);
            // Cero filas afectada ... we have problems.
            if (result == 0)
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
    public async Task<IActionResult> Put(int id, TarifasTerminal entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            /*if(id!=entity.id)
            {
                return BadRequest();
            }*/
            var result = await _unitOfWork.TarifasTerminals.UpdateAsync(entity);
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

    //[HttpDelete("{id}")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _unitOfWork.TarifasTerminals.DeleteAsync(id);
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

    [HttpGet("{contype}")]
    public async Task<ActionResult<TarifasTerminal>> Get(string description, string contype)
    {
        try
        {
            var result = await _unitOfWork.TarifasTerminals.GetByContTypeAsync(contype);
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

    [HttpGet(Name = "GetAll Terminal/Tarifas")]
    public async Task<IEnumerable<TarifasTerminal>> GetAll()
    {
        try
        {
            return await _unitOfWork.TarifasTerminals.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
