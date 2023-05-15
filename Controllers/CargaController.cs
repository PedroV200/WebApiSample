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
            var result = await _unitOfWork.Cargas.AddAsync(entity);
            // Cero filas afectada ... we have problems.
            if (result == 0)
            {
                return NotFound();
            }
            // Ok
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Carga entity)
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
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _unitOfWork.Cargas.DeleteAsync(id);
            // Ninguna fila afectada .... El id no existe
            if (result == 0)
            {
                return NotFound();
            }
            // Si llegue hasta aca, OK
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Carga>> Get(int id)
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

        [HttpGet(Name = "GetAll Carga")]
        public async Task<IEnumerable<Carga>> GetAll()
        {
            return await _unitOfWork.Cargas.GetAllAsync();
        }

    }
}
