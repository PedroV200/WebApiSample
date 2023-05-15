namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization; 

public class EstimateHeaderRepository : IEstimateHeaderRepository
{
    private readonly IConfiguration configuration;
    public EstimateHeaderRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(EstimateHeader entity)
    {
        var sql = $"INSERT INTO p_estimateheaders (own, description, articlefamily, oemsupplier, ivaexcento, dolarbillete, freighttype, freightweight, freightvolume, htimestamp) VALUES ('{entity.own}','{entity.description}','{entity.articlefamily}','{entity.oemsupplier}',{entity.ivaexcento},'{entity.dollarBillete.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.freighttype}','{entity.freightweight.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.freightweight.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',{entity.htimestamp})";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int code)
    {
        var sql = $"DELETE FROM p_estimateheaders WHERE code = {code}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<EstimateHeader>>GetAllAsync()
    {
        var sql = "SELECT * FROM p_estimateheaders";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<EstimateHeader>(sql);
        }
    }
    public async Task<EstimateHeader> GetByIdAsync(int code)
    {
        var sql = $"SELECT * FROM p_estimateheaders WHERE code = {code}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<EstimateHeader>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(EstimateHeader entity)
    {
        
        var sql = @"UPDATE p_estimateheaders SET 
                    own = @own, 
                    description = @description, 
                    articlefamily = @articlefamily, 
                    oemsupplier = @oemsupplier, 
                    ivaexcento = @ivaexcento,
                    dolarbillete = @dolarbillete,
                    freighttype = @freighttype,
                    freightweight = @freightweight,
                    freightvolume = @freightvalume,
                    htimestamp = @htimestamp
                             WHERE code = @code";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}