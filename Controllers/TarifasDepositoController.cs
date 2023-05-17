using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class TarifasDepositoController : ControllerBase
{
    private readonly ILogger<TarifasDepositoController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TarifasDepositoController(ILogger<TarifasDepositoController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Deposito/tarifa")]
    public async Task<IActionResult>Post(TarifasDeposito entity)
    {
        var result=await _unitOfWork.TarifasDepositos.AddAsync(entity);
        // Cero filas afectada ... we have problems.
        if(result==0)
        {
            return NotFound();
        }
        // Ok
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>Put(int id,TarifasDeposito entity)
    {
        // Controlo que el id sea consistente.
        /*if(id!=entity.id)
        {
            return BadRequest();
        }*/
        var result=await _unitOfWork.TarifasDepositos.UpdateByDepoContTypeAsync(entity);
        // Si la operacion devolvio 0 filas .... es por que no le pegue al id.
        if(result==0)
        {
            return NotFound();
        }
        // Si llegue hasta aca ... OK
        return Ok(result);
    }

    //[HttpDelete("{id}")]
    [HttpDelete("{dep}/{cont}")]
    public async Task<IActionResult>Delete(string dep, string cont)
    {
        var result=await _unitOfWork.TarifasDepositos.DeleteByDepoContTypeAsync(dep,cont);
        // Ninguna fila afectada .... El id no existe
        if(result==0)
        {
            return NotFound();
        }
        // Si llegue hasta aca, OK
        return Ok(result);
    }

    [HttpGet("{dep}/{cont}")]
    public async Task<ActionResult<TarifasDeposito>> Get(string dep,string cont)
    {
        var result=await _unitOfWork.TarifasDepositos.GetByDepoContTypeAsync(dep,cont);
        if(result==null)
        {
            return NotFound();
        }
        else
        {
            return result;
        }
    }

    [HttpGet(Name = "GetAll Depositos/Tarifas")]
    public async Task<IEnumerable<TarifasDeposito>> GetAll()
    {
        return await _unitOfWork.TarifasDepositos.GetAllAsync();
    }
}
