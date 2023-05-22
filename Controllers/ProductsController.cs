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
        try
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
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult>Put(int id,Product entity)
    {
        try
        {
            // Controlo que el id sea consistente.
            if (id!=entity.Id)
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
            var result=await _unitOfWork.Products.DeleteAsync(id);
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
    public async Task<Product> Get(int id)
    {
        try
        {
            return await _unitOfWork.Products.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet(Name = "GetAll")]
    public async Task<IEnumerable<Product>> GetAll()
    {
        try
        {
            return await _unitOfWork.Products.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
