namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class CanalRepository : ICanalRepository
{
 private readonly IConfiguration configuration;
    public CanalRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(Canal entity)
    {
        try
        {
            var sql = $"INSERT INTO canales (description) VALUES ('{entity.description}')";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
    }catch (Exception)
        {
            return -1;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        try
        {
            var sql = $"DELETE FROM canales WHERE id = {id}";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql);

                return result;
            }
        }catch (Exception)
        {
            return -1;
        }
    }
    public async Task<IEnumerable<Canal>> GetAllAsync()
    {
        var sql = "SELECT * FROM canales";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<Canal>(sql);
        }
    }
    public async Task<Canal> GetByIdAsync(int id)
    {
            var sql = $"SELECT * FROM canales WHERE id = {id}";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<Canal>(sql);
                return result;
            }
    }
    public async Task<int> UpdateAsync(Canal entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE canales SET description = @description WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}