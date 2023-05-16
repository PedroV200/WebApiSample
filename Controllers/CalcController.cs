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
        return await myCalc.calculateCBM();
    }

    

   
}
