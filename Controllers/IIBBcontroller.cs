using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class IIBBcontroller : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public IIBBcontroller(ILogger<ProductsController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post IIBB")]
    public async Task<IActionResult>Post(IIBB entity)
    {
        try
        {
            var result=await _unitOfWork.IIBBs.AddAsync(entity);
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

    [HttpPut("{code}")]
    public async Task<IActionResult>Put(int code,IIBB entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            if (code!=entity.code)
            {
                return BadRequest();
            }
            var result=await _unitOfWork.IIBBs.UpdateAsync(entity);
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

    [HttpDelete("{code}")]
    public async Task<IActionResult>Delete(int code)
    {
        try
        {
            var result=await _unitOfWork.IIBBs.DeleteAsync(code);
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

    [HttpGet("{code}")]
    public async Task<IIBB> Get(int code)
    {
        try
        {
            return await _unitOfWork.IIBBs.GetByIdAsync(code);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet(Name = "GetAll_IIBB")]
    public async Task<IEnumerable<IIBB>> GetAll()
    {
        try
        {
            return await _unitOfWork.IIBBs.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
