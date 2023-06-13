namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class CnstService: ICnstService
{
    IUnitOfWork _unitOfWork;
    public CnstService(IUnitOfWork unitOfWork)
    {
       _unitOfWork=unitOfWork;
    }
// Pide la tabla de constantes y la carga en un objeto CONSTATES para que Estimate y EstimateDetail puedan usarlo
    public async Task<CONSTANTES> getConstantes()
    {
        List<Cnst> tablaConstantes=new List<Cnst>();
        CONSTANTES misConstantes=new CONSTANTES();
        var tmp=await _unitOfWork.Constantes.GetAllAsync();
        tablaConstantes=tmp.ToList();
        foreach(Cnst cons in tablaConstantes)
        {
            switch(cons.description)
            {
                case "CNST_GASTOS_DESPA_Cif_Min":       misConstantes.CNST_GASTOS_DESPA_Cif_Min=cons.val;     break;
                case "CNST_GASTOS_DESPA_Cif_Mult":      misConstantes.CNST_GASTOS_DESPA_Cif_Mult=cons.val;    break;
                case "CNST_GASTOS_DESPA_Cif_Thrhld":    misConstantes.CNST_GASTOS_DESPA_Cif_Thrhld=cons.val;  break;
                case "CNST_GASTOS_CUSTODIA_Thrshld":    misConstantes.CNST_GASTOS_CUSTODIA_Thrshld=cons.val;  break;
                case "CNST_GASTOS_GETDIGDOC_Mult":      misConstantes.CNST_GASTOS_GETDIGDOC_Mult=cons.val;    break;
                case "CNST_GASTOS_BANCARIOS_Mult":      misConstantes.CNST_GASTOS_BANCARIOS_Mult=cons.val;    break;
                case "CONST_NCM_DIE_Min":               misConstantes.CONST_NCM_DIE_Min=cons.val;             break;
                case "CNST_ESTAD061_ThrhldMAX":         misConstantes.CNST_ESTAD061_ThrhldMAX=cons.val;       break;
                case "CNST_ESTAD061_ThrhldMIN":         misConstantes.CNST_ESTAD061_ThrhldMIN=cons.val;       break;
                case "CNST_GCIAS_424_Mult":             misConstantes.CNST_GCIAS_424_Mult=cons.val;           break;
                default: break;
            }       
        }
        return misConstantes;
    }
}