using Microsoft.AspNetCore.Mvc;
using WebApiSample.Models;
using WebApiSample.Infrastructure;
using WebApiSample.Core;
namespace WebApiSample.Controllers;

[ApiController]
[Route("[controller]")]
public class CalcController : ControllerBase
{
    private readonly ILogger<CalcController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEstimateService _estService;

    private calc myCalc;

    public CalcController(ILogger<CalcController> logger, IUnitOfWork unitOfWork,IEstimateService estService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _estService = estService;
        myCalc = new calc(_unitOfWork,_estService);
    }

    [HttpPost(Name = "Post Estimate")]
    public async Task<int>Post(EstimateDB entity)
    {
        var result=0;

        EstimateHeaderDB readBackHeader=new EstimateHeaderDB();
        
        readBackHeader=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(entity.estHeaderDB.EstNumber,entity.estHeaderDB.EstVers);
        if(readBackHeader !=null)
        {
            return -1;
        }

        result=await _unitOfWork.EstimateHeadersDB.AddAsync(entity.estHeaderDB);
        // Hayun probñema aca. Acabo de insertar un header pero no se que Id aisgno la base
        // ya que es autoID PK.
        // No me queda otra que leer de la base el header ingresado para descubrir su id
        readBackHeader=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(entity.estHeaderDB.EstNumber,entity.estHeaderDB.EstVers);
        // Ahora si, inserto los detail uno a uno.
        result=0;
        foreach(EstimateDetailDB ed in entity.estDetailsDB)
        {
            ed.IdEstHeader=readBackHeader.Id; // El ID que la base le asigno al header que acabo de insertar.
            result+=await _unitOfWork.EstimateDetailsDB.AddAsync(ed);
        }
        //return await myCalc.calcBatch(entity.estHeaderDB.Id);
        return result;
    }

    
   [HttpGet("{id}")]
    public async Task<EstimateDB>Post(int id)
    {
        dbutils dbHelper=new dbutils(_unitOfWork);
        return await dbHelper.getEstimateLastVers(id);
    }

    [HttpDelete("{estNumber}/{estVers}")]
    public async Task<int>Delete(int estNumber,int estVers)
    {
        dbutils dbHelper=new dbutils(_unitOfWork);
        return await dbHelper.deleteEstimateByNumByVers(estNumber,estVers);
    }
   
}
