using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class TarifasFwdContController : ControllerBase
{
    private readonly ILogger<TarifasFwdContController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TarifasFwdContController(ILogger<TarifasFwdContController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Tarifas Fowarders / contenedor")]
    public async Task<IActionResult>Post(TarifasFwdCont entity)
    {
        var result=await _unitOfWork.TarifasFwdContenedores.AddAsync(entity);
        // Cero filas afectada ... we have problems.
        if(result==0)
        {
            return NotFound();
        }
        // Ok
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>Put(int id,TarifasFwdCont entity)
    {
        // Controlo que el id sea consistente.
        /*if(id!=entity.id)
        {
            return BadRequest();
        }*/
        var result=await _unitOfWork.TarifasFwdContenedores.UpdateByFwdContTypeAsync(entity);
        // Si la operacion devolvio 0 filas .... es por que no le pegue al id.
        if(result==0)
        {
            return NotFound();
        }
        // Si llegue hasta aca ... OK
        return Ok(result);
    }

    //[HttpDelete("{id}")]
    [HttpDelete("{fwd}/{cont}")]
    public async Task<IActionResult>Delete(string fwd, string cont)
    {
        var result=await _unitOfWork.TarifasFwdContenedores.DeleteByFwdContTypeAsync(fwd,cont);
        // Ninguna fila afectada .... El id no existe
        if(result==0)
        {
            return NotFound();
        }
        // Si llegue hasta aca, OK
        return Ok(result);
    }

    [HttpGet("{fwd}/{cont}")]
    public async Task<ActionResult<TarifasFwdCont>> Get(string fwd,string cont)
    {
        var result=await _unitOfWork.TarifasFwdContenedores.GetByFwdContTypeAsync(fwd,cont);
        if(result==null)
        {
            return NotFound();
        }
        else
        {
            return result;
        }
    }

    [HttpGet(Name = "GetAll Fowarder / tarifas")]
    public async Task<IEnumerable<TarifasFwdCont>> GetAll()
    {
        return await _unitOfWork.TarifasFwdContenedores.GetAllAsync();
    }
}
