namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class ImpuestoRepository : IImpuestoRepository
{
 private readonly IConfiguration configuration;
    public ImpuestoRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(Impuesto entity)
    {
        var sql = $"INSERT INTO impuestos (description) VALUES ('{entity.description}')";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM impuestos WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<Impuesto>> GetAllAsync()
    {
        var sql = "SELECT * FROM impuestos";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<Impuesto>(sql);
        }
    }
    public async Task<Impuesto> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM impuestos WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Impuesto>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(Impuesto entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE impuestos SET description = @description WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}