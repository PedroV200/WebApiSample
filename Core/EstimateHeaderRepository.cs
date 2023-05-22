namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization; 

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
        var sql = $"INSERT INTO estimateheader (Description, EstNumber, EstVers, Own, ArticleFamily, OemSupplier, IvaExcento, DolarBillete, FreightType, FreightFwd, FobGrandTotal, FleteTotal, SeguroPorct, Seguro, CantidadContenedores, Pagado, htimestamp) VALUES ('{entity.Description}',{entity.EstNumber},{entity.EstVers},'{entity.Own}','{entity.ArticleFamily}','{entity.OemSupplier}',{entity.IvaExcento},'{entity.DollarBillete.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.FreightType}','{entity.FreightFwd}','{entity.FobGrandTotal.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.FleteTotal.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.Seguro.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.SeguroPorct.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.CantidadContenedores.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.Pagado.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{tmpString}')";
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
    public async Task<IEnumerable<EstimateHeaderDB>> GetByEstNumberLastVersAsync(int estNumber)
    {
        var sql = $"SELECT * FROM estimateheader WHERE EstNumber=estNumber ORDER BY EstVers DESC";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            return await connection.QueryAsync<EstimateHeaderDB>(sql);
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
                    OemSupplier = @OemSupplier, 
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