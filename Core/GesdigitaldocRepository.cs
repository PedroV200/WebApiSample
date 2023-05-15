namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class GestdigitaldocRepository : IGestdigitaldocRepository
{
 private readonly IConfiguration configuration;
    public GestdigitaldocRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(Gestdigitaldoc entity)
    {
        var sql = $"INSERT INTO gestdigdoc (description) VALUES ('{entity.description}')";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM gestdigdoc WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<Gestdigitaldoc>> GetAllAsync()
    {
        var sql = "SELECT * FROM gestdigdoc";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<Gestdigitaldoc>(sql);
        }
    }
    public async Task<Gestdigitaldoc> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM gestdigdoc WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Gestdigitaldoc>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(Gestdigitaldoc entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE gestdigdoc SET description = @description WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}