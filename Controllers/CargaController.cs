using Microsoft.AspNetCore.Mvc;
using WebApiSample.Infrastructure;
using WebApiSample.Models;

namespace WebApiSample.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class CargaController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CargaController(ILogger<ProductsController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpPost(Name = "Post Carga")]
        public async Task<IActionResult> Post(Carga entity)
        {
            try
            {
                var result = await _unitOfWork.Cargas.AddAsync(entity);
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
        public async Task<IActionResult> Put(int id, Carga entity)
        {
            try
            {
                // Controlo que el id sea consistente.
                if (id != entity.id)
                {
                    return BadRequest();
                }
                var result = await _unitOfWork.Cargas.UpdateAsync(entity);
                // Si la operacion devolvio 0 filas .... es por que no le pegue al id.
                if (result == 0)
                {
                    return NotFound();
                }
                // Si llegue hasta aca ... OK
                return Ok(result);
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _unitOfWork.Cargas.DeleteAsync(id);
                // Ninguna fila afectada .... El id no existe
                if (result == 0)
                {
                    return NotFound();
                }
                // Si llegue hasta aca, OK
                return Ok(result);
            }catch(Exception)
            {
                throw new Exception($"Could not delete {id}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Carga>> Get(int id)
        {
            try
            {
                var result = await _unitOfWork.Cargas.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound();
                }
                else
                {
                    return result;
                }
            }
            catch (Exception)
            {
                throw new Exception($"No existe Carga con Id {id}");
            }
        }

        [HttpGet(Name = "GetAll Carga")]
        public async Task<IEnumerable<Carga>> GetAll()
        {
            try
            {
                return await _unitOfWork.Cargas.GetAllAsync();
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
