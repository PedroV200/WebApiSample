namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class TipoDeCambioRepository : ITipoDeCambioRepository
{
    private readonly IConfiguration configuration;
    public TipoDeCambioRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(TipoDeCambio entity)
    {
        var sql = $"INSERT INTO TC_CDA (description, Day) VALUES (@description, @day)";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, new
            {
                description = entity.description, day = entity.Day
            });
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM TC_CDA WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<TipoDeCambio>> GetAllAsync()
    {
        var sql = "SELECT * FROM TC_CDA";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TipoDeCambio>(sql);
        }
    }
    public async Task<TipoDeCambio> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM TC_CDA WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TipoDeCambio>(sql);
            return result;
        }
    }

    public async Task<double> GetByDateAsync(string date)
    {
        var sql = $"SELECT MAX(description) FROM tc_cda WHERE day >= '{date}'::date AND day < ('{date}'::date + '1 day'::interval)";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            try
            {
                var result = await connection.QuerySingleOrDefaultAsync<double>(sql);
                return result;
            }
            catch
            {
                return -1;
            }
        }
    }
    
    public async Task<int> UpdateAsync(TipoDeCambio entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE TC_CDA SET description = @description WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}