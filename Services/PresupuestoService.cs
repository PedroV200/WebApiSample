namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;


// LISTED 22_6_2023 15:59 PM
public class PresupuestoService:IPresupuestoService
{
    
    private calc myCalc;

    IUnitOfWork _unitOfWork;

    IEstimateService _estService;

    public PresupuestoService(IUnitOfWork unitOfWork, IEstimateService estService)
    {
        _unitOfWork=unitOfWork;
        _estService=estService;
         myCalc = new calc(_unitOfWork,_estService);
    }

    public string getLastErr()
    {
        return myCalc.haltError;
    }

    public async Task<EstimateV2>submitPresupuesto(EstimateDB miEst)
    {
        var result=0;
        EstimateV2 ret=new EstimateV2();

        // Cuando me pasan un presupuesto con VERSION 0, significa que es una simulacion
        // y no se ingresara a la base.
        if(miEst.estHeaderDB.EstNumber==0)
        {
            ret=await myCalc.calcBatch(miEst);
            return ret;
        }

        // La version no es 0. No es una simulacion. Va en serio. 
        EstimateHeaderDB readBackHeader=new EstimateHeaderDB();
        
        readBackHeader=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(miEst.estHeaderDB.EstNumber,miEst.estHeaderDB.EstVers);
        if(readBackHeader !=null)
        {   // OJO
             return null;
        }

        result=await _unitOfWork.EstimateHeadersDB.AddAsync(miEst.estHeaderDB);
        // Hayun probñema aca. Acabo de insertar un header pero no se que Id aisgno la base
        // ya que es autoID PK.
        // No me queda otra que leer de la base el header ingresado para descubrir su id
        readBackHeader=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(miEst.estHeaderDB.EstNumber,miEst.estHeaderDB.EstVers);
        // Ahora si, inserto los detail uno a uno.
            
        foreach(EstimateDetailDB ed in miEst.estDetailsDB)
        {
            ed.IdEstHeader=readBackHeader.Id; // El ID que la base le asigno al header que acabo de insertar.
            result+=await _unitOfWork.EstimateDetailsDB.AddAsync(ed);
        }
        ret=await myCalc.calcBatch(miEst);

        return ret;
        
    }

}