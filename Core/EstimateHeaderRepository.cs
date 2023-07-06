namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization; 


// LISTED 29_6_2023 17:56 
//REFACTOR agrega IDs (FKs a los maestros) para todos los proveedores (servicios u OEM) 
//REFACTOR para tratar al proveedor de poliza igual que al resto de los proveedores (descrip / ID)



public class EstimateHeaderDBRepository : IEstimateHeaderDBRepository
{
    private readonly IConfiguration configuration;
    public EstimateHeaderDBRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(EstimateHeaderDB entity)
    {
        // Convierto la fecha al formato que postgre acepta. Le molesta AAAA/MM//dd. Tiene que ser AAAA-MM-dd
        //entity.hTimeStamp=DateTime.Now;
        string tmpString=entity.hTimeStamp.ToString("yyyy-MM-dd hh:mm:ss");
        //entity.hTimeStamp=DateOnly.FromDateTime(DateTime.Now);
        var sql = $@"INSERT INTO estimateheader 
                (Description, 
                EstNumber, 
                EstVers, 
                Own, 
                ArticleFamily, 
                IvaExcento,
                FreightType, 
                FreightFwd, 
                FobGrandTotal, 
                FleteTotal, 
                Seguro, 
                CantidadContenedores, 
                Pagado, 
                    CbmTot, 
	                CifTot,
                    IibbTot,
                    ExtraGastosLocProyectado, 
                    c_gdespa_cif_min,
                    c_gdespa_cif_mult, 
                    c_gdespa_cif_thrhld, 
                    c_gcust_thrshld,
                    c_ggesdoc_mult,
                    c_gbanc_mult,
                    c_ncmdie_min,
                    c_est061_thrhldmax, 
                    c_est061_thrhldmin, 
                        c_gcias424_mult, 
                        c_seguroporct,
                    c_arancelsim,
                    DolarBillete, 
                    PolizaProv,
                    p_gloc_banco,
                    p_gloc_fwder,  
                    p_gloc_term, 
                    p_gloc_despa,
                    p_gloc_tte, 
                    p_gloc_cust,
                    p_gloc_gestdigdoc,
                    oemprove1,   
                    oemprove2,
                    oemprove3,   
                    oemprove4,
                    oemprove5,   
                    oemprove6,
                    oemprove7,   
                    id_polizaprov,
                    id_p_gloc_banco,
                    id_p_gloc_fwder,  
                    id_p_gloc_term, 
                    id_p_gloc_despa,
                    id_p_gloc_tte, 
                    id_p_gloc_cust,
                    id_p_gloc_gestdigdoc,
                    id_oemprove1,   
                    id_oemprove2,
                    id_oemprove3,   
                    id_oemprove4,
                    id_oemprove5,   
                    id_oemprove6,
                    id_oemprove7,   

                htimestamp) 
                            VALUES 
                                    ('{entity.Description}',
                                    {entity.EstNumber},
                                    {entity.EstVers},
                                    '{entity.Own}',
                                    '{entity.ArticleFamily}',
                                    '{entity.IvaExcento}',
                                    '{entity.FreightType}',
                                    '{entity.FreightFwd}',
                                    '{entity.FobGrandTotal.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.FleteTotal.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    
                                    '{entity.Seguro.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    
                                    '{entity.CantidadContenedores.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.Pagado.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.CbmTot.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.CifTot.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.IibbTot.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.ExtraGastosLocProyectado.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.c_gdespa_cif_min.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.c_gdespa_cif_mult.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.c_gdespa_cif_thrhld.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.c_gcust_thrshld.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.c_ggesdoc_mult.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.c_gbanc_mult.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.c_ncmdie_min.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.c_est061_thrhldmax.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.c_est061_thrhldmin.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.c_gcias424_mult.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.c_seguroporct.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.c_arancelsim.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.DolarBillete.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.PolizaProv}',
                                    '{entity.p_gloc_banco}',
                                    '{entity.p_gloc_fwder}',
                                    '{entity.p_gloc_term}',
                                    '{entity.p_gloc_despa}',
                                    '{entity.p_gloc_tte}',
                                    '{entity.p_gloc_cust}',
                                    '{entity.p_gloc_gestdigdoc}',
                                    '{entity.oemprove1}',
                                    '{entity.oemprove2}',
                                    '{entity.oemprove3}',
                                    '{entity.oemprove4}',
                                    '{entity.oemprove5}',
                                    '{entity.oemprove6}',
                                    '{entity.oemprove7}',

                                    {entity.id_PolizaProv},
                                    {entity.id_p_gloc_banco},
                                    {entity.id_p_gloc_fwder},
                                    {entity.id_p_gloc_term},
                                    {entity.id_p_gloc_despa},
                                    {entity.id_p_gloc_tte},
                                    {entity.id_p_gloc_cust},
                                    {entity.id_p_gloc_gestdigdoc},
                                    {entity.id_oemprove1},
                                    {entity.id_oemprove2},
                                    {entity.id_oemprove3},
                                    {entity.id_oemprove4},
                                    {entity.id_oemprove5},
                                    {entity.id_oemprove6},
                                    {entity.id_oemprove7},
                                    '{tmpString}')";

 

        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int Id)
    {
        var sql = $"DELETE FROM estimateheader WHERE Id = {Id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<EstimateHeaderDB>>GetAllAsync()
    {
        var sql = "SELECT * FROM estimateheader";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<EstimateHeaderDB>(sql);
        }
    }
    public async Task<EstimateHeaderDB> GetByIdAsync(int Id)
    {
        var sql = $"SELECT * FROM estimateheader WHERE Id = {Id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<EstimateHeaderDB>(sql);
            return result;
        }
    }
    public async Task<IEnumerable<EstimateHeaderDB>> GetByEstNumberLastVersAsync(int estnumber)
    {
        var sql = $"SELECT * FROM estimateheader WHERE estnumber={estnumber} ORDER BY estvers DESC";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            return await connection.QueryAsync<EstimateHeaderDB>(sql);
        }
    }

    // 6/7/2023 Trae el proximo ID LIBRE para estNumber.
    public async Task<int> GetNextEstNumber()
    {
        var sql = $"SELECT MAX(estnumber) from estimateheader";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            return (await connection.QuerySingleOrDefaultAsync<int>(sql)+1);
        }
    }

    // 6/7/2023 Trae la proxima version LIBRE de un determinado presupuesto
    public async Task<int> GetNextEstVersByEstNumber(int estNumber)
    {
        var sql = $"select MAX(estVers) from estimateheader where estnumber={estNumber}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            return (await connection.QuerySingleOrDefaultAsync<int>(sql)+1);
        }
    }

    // 6/7/2023. Trae todos las versiones de un presupuesto
    public async Task<IEnumerable<EstimateHeaderDB>> GetAllVersionsFromEstimate(int estNumber)
    {
        var sql = $"SELECT * from estimateheader where estnumber={estNumber}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            return await connection.QueryAsync<EstimateHeaderDB>(sql);
        }
    }

    public async Task<EstimateHeaderDB> GetByEstNumberAnyVersAsync(int estnumber, int estVers)
    {
        var sql = $"SELECT * FROM estimateheader WHERE estnumber={estnumber} AND estVers={estVers}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            return await connection.QuerySingleOrDefaultAsync<EstimateHeaderDB>(sql);
        }
    }

    public async Task<int> UpdateAsync(EstimateHeaderDB entity)
    {
        
        var sql = @"UPDATE estimateheader SET 
                   

                    description = @description, 
                    EstNumber = @EstNumber,
                    EstVers = @EstVers,
                    own = @own, 
                    ArticleFamily = @articlefamily, 
                    IvaExcento = @IvaExcento,
                    FreightType = @FreightType,
                    FreightFwd = @FreightFwd,
                    FobGrandTotal = @FobGrandTotal,
                    FleteTotal = @FleteTotal,
                    Seguro = @Seguro,                   
                    CantidadContenedores = @CantidadContenedores,
                    Pagado = @Pagado,
                    CbmTot = @CbmTot, 
	                CifTot= @CifTot,
                    IibbTot= @IibbTot,
                    ExtraGastosLocProyectado= @ExtraGastosLocProyectado, 
                    c_gdespa_cif_min= @c_gdespa_cif_min,
                    c_gdespa_cif_mult=  @c_gdespa_cif_mult, 
                    c_gdespa_cif_thrhld= @c_gdespa_cif_thrhld, 
                    c_gcust_thrshld= @c_gcust_thrshld,
                    c_ggesdoc_mult=  @c_ggesdoc_mult,
                    c_gbanc_mult= @c_gbanc_mult,
                    c_ncmdie_min= @c_ncmdie_min,
                    c_est061_thrhldmax= @c_est061_thrhldmax, 
                    c_est061_thrhldmin= @c_est061_thrhldmin, 
                    c_gcias424_mult= @c_gcias424_mult, 
                    c_seguroporct = @seguroporct,
                    c_arancelsim = @c_arancelsim,
                    DolarBillete = @DolarBillete,
                    PolizaProv = @PolizaProv,
                    p_gloc_banco= @p_gloc_banco,
                    p_gloc_fwder=@p_gloc_fwder,  
                    p_gloc_term= @p_gloc_term, 
                    p_gloc_despa= @p_gloc_despa,
                    p_gloc_tte=@p_gloc_tte, 
                    p_gloc_cust= @p_gloc_cust,
                    p_gloc_gestdigdoc=@p_gloc_gestdigdoc,
                    oemprove1=@oemprove1,
                    oemprove1=@oemprove2,
                    oemprove1=@oemprove3,
                    oemprove1=@oemprove4,
                    oemprove1=@oemprove5,
                    oemprove1=@oemprove6,
                    oemprove1=@oemprove7,

                    id_polizaprov=@id_polizaprov,
                    id_p_gloc_banco= @id_p_gloc_banco,
                    id_p_gloc_fwder=@id_p_gloc_fwder,  
                    id_p_gloc_term= @id_p_gloc_term, 
                    id_p_gloc_despa= @id_p_gloc_despa,
                    id_p_gloc_tte=@id_p_gloc_tte, 
                    id_p_gloc_cust= @id_p_gloc_cust,
                    id_p_gloc_gestdigdoc=@id_p_gloc_gestdigdoc,
                    id_oemprove1=@id_oemprove1,
                    id_oemprove1=@id_oemprove2,
                    id_oemprove1=@id_oemprove3,
                    id_oemprove1=@id_oemprove4,
                    id_oemprove1=@id_oemprove5,
                    id_oemprove1=@id_oemprove6,
                    id_oemprove1=@id_oemprove7,

                        hTimeStamp = @hTimeStamp
                             WHERE Id = @Id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}