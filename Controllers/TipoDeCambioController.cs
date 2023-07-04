using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class TipoDeCambioController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TipoDeCambioController(ILogger<ProductsController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post Tipo de Cambio")]
    public async Task<IActionResult> Post(TipoDeCambio entity)
    {
        try
        {
        var result = await _unitOfWork.TiposDeCambio.AddAsync(entity);
        // Cero filas afectada ... we have problems.
        if (result == 0)
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
    public async Task<IActionResult> Put(int id, TipoDeCambio entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            if (id != entity.id)
            {
                return BadRequest();
            }
            var result = await _unitOfWork.TiposDeCambio.UpdateAsync(entity);
            // Si la operacion devolvio 0 filas .... es por que no le pegue al id.
            if (result == 0)
            {
                return NotFound();
            }
            // Si llegue hasta aca ... OK
            return Ok(result);
        }catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _unitOfWork.TiposDeCambio.DeleteAsync(id);
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
    public async Task<ActionResult<TipoDeCambio>> Get(int id)
    {
        try
        {
            
            var result = await _unitOfWork.TiposDeCambio.GetByIdAsync(id);
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

    [HttpGet(Name = "GetAll Tipo de Cambio")]
    public async Task<IEnumerable<TipoDeCambio>> GetAll()
    {
        try
        { 
            //Testeado OK
            //string hoy=DateTime.Now.ToString("yyyy-MM-dd");
            //double tipoDeCambio= await _unitOfWork.TiposDeCambio.GetByDateAsync("2023-07-07"); 
            return await _unitOfWork.TiposDeCambio.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
