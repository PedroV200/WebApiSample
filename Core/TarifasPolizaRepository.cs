namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

// LISTED 12_06_2023 10:25AM

public class TarifasPolizaRepository : ITarifasPolizaRepository
{
 private readonly IConfiguration configuration;
    public TarifasPolizaRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(TarifasPoliza entity)
    {
        var sql = $"INSERT INTO tarifaspoliza (description, prima, demora, impint, sellos) VALUES ('{entity.description}','{entity.prima.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.demora.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.impint.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.sellos.ToString(CultureInfo.CreateSpecificCulture("en-US"))}')";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM tarifaspoliza WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<IEnumerable<TarifasPoliza>> GetAllAsync()
    {
        var sql = "SELECT * FROM tarifaspoliza";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasPoliza>(sql);
        }
    }
    public async Task<TarifasPoliza> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM tarifaspoliza WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasPoliza>(sql);
            return result;
        }
    }

        public async Task<TarifasPoliza> GetByDescAsync(string proveedor)
    {
        var sql = $"SELECT * FROM tarifaspoliza WHERE description = '{proveedor}'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasPoliza>(sql);
            return result;
        }
    }

    public async Task<int> UpdateAsync(TarifasPoliza entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE tarifaspoliza SET 
                    description = @description,
                    prima = @prima, 
                    demora = @demora, 
                    impint = @impint, 
                    sellos = @sellos
                             WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}