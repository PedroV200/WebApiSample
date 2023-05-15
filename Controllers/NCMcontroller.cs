using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class NCMcontroller : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public NCMcontroller(ILogger<ProductsController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post NCM")]
    public async Task<IActionResult>Post(NCM entity)
    {
        var result=await _unitOfWork.NCMs.AddAsync(entity);
        // Cero filas afectada ... we have problems.
        if(result==0)
        {
            return NotFound();
        }
        // Ok
        return Ok();
    }

    [HttpPut("{code}")]
    public async Task<IActionResult>Put(string code,NCM entity)
    {
        // Controlo que el id sea consistente.
        if(code!=entity.code)
        {
            return BadRequest();
        }
        var result=await _unitOfWork.NCMs.UpdateAsync(entity);
        // Si la operacion devolvio 0 filas .... es por que no le pegue al id.
        if(result==0)
        {
            return NotFound();
        }
        // Si llegue hasta aca ... OK
        return Ok(result);
    }

    [HttpDelete("{code}")]
    public async Task<IActionResult>Delete(string code)
    {
        var result=await _unitOfWork.NCMs.DeleteByStrAsync(code);
        // Ninguna fila afectada .... El id no existe
        if(result==0)
        {
            return NotFound();
        }
        // Si llegue hasta aca, OK
        return Ok(result);
    }

    [HttpGet("{code}")]
    public async Task<NCM> Get(string code)
    {
        return await _unitOfWork.NCMs.GetByIdStrAsync(code);
    }

    [HttpGet(Name = "GetAll_NCM")]
    public async Task<IEnumerable<NCM>> GetAll()
    {
        return await _unitOfWork.NCMs.GetAllAsync();
    }
}
