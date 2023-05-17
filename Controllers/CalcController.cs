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

    private calc myCalc;

    public CalcController(ILogger<CalcController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        //myEstimate = new Estimate(unitOfWork);
        myCalc = new calc(unitOfWork);
    }

    [HttpPost(Name = "Post Estimate")]
    public async Task<List<double>>Post(Estimate entity)
    {
        var result=0;
        result=await _unitOfWork.EstimateHeaders.AddAsync(entity.estHeader);
        foreach(EstimateDetail ed in entity.estDetails)
        {
            result=await _unitOfWork.EstimateDetails.AddAsync(ed);
        }
        return await myCalc.calcBatch(entity.estHeader.code);
    }

    
   [HttpGet("{id}")]
    public async Task<Estimate>Post(int id)
    {
        Estimate myEst=new Estimate(); 

        myEst.estHeader=await _unitOfWork.EstimateHeaders.GetByIdAsync(id);
        var result=await _unitOfWork.EstimateDetails.GetAllByCodeAsync(id);
        myEst.estDetails=result.ToList();
        return myEst;
    }
   
}
