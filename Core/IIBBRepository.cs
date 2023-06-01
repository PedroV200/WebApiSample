namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class IIBBRepository : IIIBBrepository
{
    private readonly IConfiguration configuration;
    public IIBBRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(IIBB entity)
    {
        var sql = $"INSERT INTO iibb (code, description, coef, alicuota, factor) VALUES ('{entity.code}','{entity.description}','{entity.coef.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.alicuota.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.factor.ToString(CultureInfo.CreateSpecificCulture("en-US"))}')";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int code)
    {
        var sql = $"DELETE FROM iibb WHERE code = {code}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<IIBB>> GetAllAsync()
    {
        var sql = "SELECT * FROM iibb";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<IIBB>(sql);
        }
    }

    public async Task<double> GetSumFactores()
    {
        var sql = "SELECT SUM(factor) FROM iibb";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QuerySingleOrDefaultAsync<double>(sql);
        }
    }

    public async Task<IIBB> GetByIdAsync(int code)
    {
        var sql = $"SELECT * FROM iibb WHERE code = {code}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<IIBB>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(IIBB entity)
    {
        
        var sql = @"UPDATE iibb SET 
                    code = @code, 
                    id = @id,
                    description = @description, 
                    coef = @coef, 
                    alicuota = @alicuota, 
                    factor = @factor
                             WHERE code = @code";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}