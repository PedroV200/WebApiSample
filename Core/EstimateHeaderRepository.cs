namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization; 

// LISTED 14_06_2023 11_49AM

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
        string tmpString=entity.hTimeStamp.ToString("yyyy-MM-dd hh:mm:ss");
        //entity.hTimeStamp=DateOnly.FromDateTime(DateTime.Now);
        var sql = $@"INSERT INTO estimateheader 
                (Description, 
                EstNumber, 
                EstVers, 
                Own, 
                ArticleFamily, 
                IvaExcento,
                DolarBillete, 
                FreightType, 
                FreightFwd, 
                FobGrandTotal, 
                FleteTotal, 
                SeguroPorct, 
                Seguro, 
                CantidadContenedores, 
                Pagado, 
                    CbmTot, 
	                CifTot,
                    IibbTot,
                    PolizaProv ,
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
                    p_gloc_banco,
                    p_gloc_fwder,  
                    p_gloc_term, 
                    p_gloc_despa,
                    p_gloc_tte, 
                    p_gloc_cust,
                    p_gloc_gestdigdoc,
                    oemprove1,     
                htimestamp) 
                            VALUES 
                                    ('{entity.Description}',
                                    {entity.EstNumber},
                                    {entity.EstVers},
                                    '{entity.Own}',
                                    '{entity.ArticleFamily}',
                                    '{entity.IvaExcento}',
                                    '{entity.DollarBillete.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.FreightType}',
                                    '{entity.FreightFwd}',
                                    '{entity.FobGrandTotal.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.FleteTotal.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.Seguro.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.SeguroPorct.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.CantidadContenedores.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.Pagado.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.CbmTot.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.CifTot.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.IibbTot.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',
                                    '{entity.PolizaProv}',
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
                                    '{entity.p_gloc_banco}',
                                    '{entity.p_gloc_fwder}',
                                    '{entity.p_gloc_term}',
                                    '{entity.p_gloc_despa}',
                                    '{entity.p_gloc_tte}',
                                    '{entity.p_gloc_cust}',
                                    '{entity.p_gloc_gestdigdoc}',
                                    '{entity.oemprove1}',
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
                    DolarBillete = @DolarBillete,
                    FreightType = @FreightType,
                    FreightFwd = @FreightFwd,
                    FobGrandTotal = @FobGrandTotal,
                    FleteTotal = @FleteTotal,
                    SeguroPorct = @SeguroPorct,
                    Seguro = @Seguro,
                    CantidadContenedores = @CantidadContenedores,
                    Pagado = @Pagado,
                    CbmTot = @CbmTot, 
	                CifTot= @CifTot,
                    IibbTot= @IibbTot,
                    PolizaProv = @PolizaProv,
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
                    p_gloc_banco= @p_gloc_banco,
                    p_gloc_fwder=@p_gloc_fwder,  
                    p_gloc_term= @p_gloc_term, 
                    p_gloc_despa= @p_gloc_despa,
                    p_gloc_tte=@p_gloc_tte, 
                    p_gloc_cust= @p_gloc_cust,
                    p_gloc_gestdigdoc=@p_gloc_gestdigdoc,
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