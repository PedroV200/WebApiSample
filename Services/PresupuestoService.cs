namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;


// LISTED 3_7_2023 15:02 PM
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


    public async Task<EstimateV2>acalcPresupuesto(EstimateDB miEst)
    {
            return await myCalc.aCalc(miEst,miEst.estHeaderDB.EstNumber,miEst.estHeaderDB.Seguro,miEst.estHeaderDB.p_gloc_banco,miEst.estHeaderDB.Pagado,miEst.estHeaderDB.p_gloc_cust);
    }

    public async Task<EstimateV2>submitPresupuestoNew(EstimateDB miEst)
    {
        var result=0;
        EstimateV2 ret=new EstimateV2();

        // La version no es 0. No es una simulacion. Va en serio. 
        EstimateHeaderDB readBackHeader=new EstimateHeaderDB();
        
        /*readBackHeader=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(miEst.estHeaderDB.EstNumber,miEst.estHeaderDB.EstVers);
        if(readBackHeader !=null)
        {   // OJO
             return null;
        }*/


        // Numeracion / Versionado:
        // Este es un nuevo presupuesto, con lo cual me limito a preguntar el estNumber mas alto usado
        // y luego le sumo 1. La version es 1.
        miEst.estHeaderDB.EstNumber=await _unitOfWork.EstimateHeadersDB.GetNextEstNumber();
        miEst.estHeaderDB.EstVers=1;

        ret=await myCalc.calcBatch(miEst);
        // Si los calculos fallan, no hacer nada.
        if(ret==null)
        {
            return null;
        }

        // Le pongo la fecha / hora !!!!!!
        ret.TimeStamp=DateTime.Now;

        // lo que me deuvelve la rutina de calculo es un EstimateV2, cuyo Detail es mucho mas extenso
        // En la base no se guardan calculos,  por lo que debi convertir el estimate V2 a estimate DB y guardarlo.
        dbutils myDBhelper=new dbutils(_unitOfWork);
        EstimateDB resultEDB=new EstimateDB();
        resultEDB=myDBhelper.transferDataToDBType(ret);

        // Guardo el header.
        result=await _unitOfWork.EstimateHeadersDB.AddAsync(resultEDB.estHeaderDB);
        // Veo que ID le asigno la base:
        readBackHeader=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(resultEDB.estHeaderDB.EstNumber,miEst.estHeaderDB.EstVers);
        // Ahora si, inserto los detail uno a uno ne la base
        foreach(EstimateDetailDB ed in resultEDB.estDetailsDB)
        {
            ed.IdEstHeader=readBackHeader.Id; // El ID que la base le asigno al header que acabo de insertar.
            result+=await _unitOfWork.EstimateDetailsDB.AddAsync(ed);
        }
        
        return ret;      
    }

    public async Task<EstimateV2>simulaPresupuesto(EstimateDB miEst)
    {
        EstimateV2 ret=new EstimateV2();
        ret=await myCalc.calcBatch(miEst);
        return ret;
    }

    public async Task<EstimateV2>submitPresupuestoUpdated(int estNumber,EstimateDB miEst)
    {
        var result=0;
        EstimateV2 ret=new EstimateV2();

        // Cuando me pasan un presupuesto con VERSION 0, significa que es una simulacion
        // y no se ingresara a la base.
        if(estNumber==0)
        {
            ret=await myCalc.calcBatch(miEst);
            return ret;
        } 



        // La version no es 0. No es una simulacion. Va en serio. 
        EstimateHeaderDB readBackHeader=new EstimateHeaderDB();
        
        /*readBackHeader=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(miEst.estHeaderDB.EstNumber,miEst.estHeaderDB.EstVers);
        if(readBackHeader !=null)
        {   // OJO
             return null;
        }*/

        miEst.estHeaderDB.EstNumber=estNumber;
        miEst.estHeaderDB.EstVers=await _unitOfWork.EstimateHeadersDB.GetNextEstVersByEstNumber(estNumber);

        if(miEst.estHeaderDB.EstVers==0)
        {
            return null;
        }

        ret=await myCalc.calcBatch(miEst);
        // Si los calculos fallan, no hacer nada.
        if(ret==null)
        {
            return null;
        }

        // Le pongo la fecha / hora !!!!!!
        ret.TimeStamp=DateTime.Now;

        // lo que me deuvelve la rutina de calculo es un EstimateV2, cuyo Detail es mucho mas extenso
        // En la base no se guardan calculos,  por lo que debi convertir el estimate V2 a estimate DB y guardarlo.
        dbutils myDBhelper=new dbutils(_unitOfWork);
        EstimateDB resultEDB=new EstimateDB();
        resultEDB=myDBhelper.transferDataToDBType(ret);

        // Guardo el header.
        result=await _unitOfWork.EstimateHeadersDB.AddAsync(resultEDB.estHeaderDB);
        // Veo que ID le asigno la base:
        readBackHeader=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(resultEDB.estHeaderDB.EstNumber,miEst.estHeaderDB.EstVers);
        // Ahora si, inserto los detail uno a uno ne la base
        foreach(EstimateDetailDB ed in resultEDB.estDetailsDB)
        {
            ed.IdEstHeader=readBackHeader.Id; // El ID que la base le asigno al header que acabo de insertar.
            result+=await _unitOfWork.EstimateDetailsDB.AddAsync(ed);
        }
        
        return ret;      
    }


    public async Task<EstimateV2>reclaimPresupuesto(int estNumber,int estVers)
    {
        EstimateV2 ret=new EstimateV2();



        EstimateDB miEst=new EstimateDB();

        // Levanto el header segun numero y version
        miEst.estHeaderDB=await _unitOfWork.EstimateHeadersDB.GetByEstNumberAnyVersAsync(estNumber,estVers);
        if(miEst.estHeaderDB ==null)
        {   // OJO
             return null;
        }
        // Con el ID del header levanto el estDetail.
        var result=await _unitOfWork.EstimateDetailsDB.GetAllByIdEstHeadersync(miEst.estHeaderDB.Id);
        if(result==null)
        {
            return null;
        }
        miEst.estDetailsDB=result.ToList();

        // Ya tengo mi estimateDB completo desde la tabla.
        // CalcRestore se vale del hecho de que muchos calculos ya estan resueltos en el header.
        // y aun tambien las constantes, con lo que no es preciso hacerlas de nuevo o solicitar nuevas
        // consultas a la base.
        ret=await myCalc.calcReclaim(miEst);

        // Si los calculos fallan, no hacer nada.
        if(ret==null)
        {
            return null;
        }
        
        return ret;      
    }

}