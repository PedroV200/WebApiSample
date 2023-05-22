using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using WebApiSample.Infrastructure;
using WebApiSample.Models;
namespace WebApiSample.Controllers

{
    [ApiController]
    [Route("[controller]")]
    public class EmpresaController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public EmpresaController(ILogger<ProductsController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpPost(Name = "Post Empresa")]
        public async Task<IActionResult> Post(Empresa entity)
        {
            try
            {
                var result = await _unitOfWork.Empresas.AddAsync(entity);
                // Cero filas afectada ... we have problems.
                if (result == 0)
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
        public async Task<IActionResult> Put(int id, Empresa entity)
        {
            try
            {
                // Controlo que el id sea consistente.
                if (id != entity.id)
                {
                    return BadRequest();
                }
                var result = await _unitOfWork.Empresas.UpdateAsync(entity);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
            var result = await _unitOfWork.Empresas.DeleteAsync(id);
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
        public async Task<ActionResult<Empresa>> Get(int id)
        {
            try
            {
                var result = await _unitOfWork.Empresas.GetByIdAsync(id);
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

        [HttpGet(Name = "GetAll Empresa")]
        public async Task<IEnumerable<Empresa>> GetAll()
        {
            try
            {
                return await _unitOfWork.Empresas.GetAllAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
