using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ProductsController(ILogger<ProductsController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpPost(Name = "Post")]
    public async Task<IActionResult>Post(Product entity)
    {
        var result=await _unitOfWork.Products.AddAsync(entity);
        // Cero filas afectada ... we have problems.
        if(result==0)
        {
            return NotFound();
        }
        // Ok
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>Put(int id,Product entity)
    {
        // Controlo que el id sea consistente.
        if(id!=entity.Id)
        {
            return BadRequest();
        }
        var result=await _unitOfWork.Products.UpdateAsync(entity);
        // Si la operacion devolvio 0 filas .... es por que no le pegue al id.
        if(result==0)
        {
            return NotFound();
        }
        // Si llegue hasta aca ... OK
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult>Delete(int id)
    {
        var result=await _unitOfWork.Products.DeleteAsync(id);
        // Ninguna fila afectada .... El id no existe
        if(result==0)
        {
            return NotFound();
        }
        // Si llegue hasta aca, OK
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<Product> Get(int id)
    {
        return await _unitOfWork.Products.GetByIdAsync(id);
    }

    [HttpGet(Name = "GetAll")]
    public async Task<IEnumerable<Product>> GetAll()
    {
        return await _unitOfWork.Products.GetAllAsync();
    }
}
