namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class TarifasTerminalRepository : ITarifasTerminalRepository
{
    private readonly IConfiguration configuration;
    public TarifasTerminalRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(TarifasTerminal entity)
    {
        var sql = $"INSERT INTO tarifasterminals (description, contype, gastofijo, gastovariable ) VALUES " +
            $"('{entity.description}'," +
            $"'{entity.contype}'," +
            $"'{entity.gastoFijo.ToString(CultureInfo.CreateSpecificCulture("en-US"))}'," +
            $"'{entity.gastoVariable.ToString(CultureInfo.CreateSpecificCulture("en-US"))}')";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM tarifasterminals WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<int> DeleteByContTypeAsync(string cont)
    {
        var sql = $"DELETE FROM tarifasterminals WHERE contype = '{cont}'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<TarifasTerminal>> GetAllAsync()
    {
        var sql = "SELECT * FROM tarifasterminals";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasTerminal>(sql);
        }
    }
    public async Task<TarifasTerminal> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM tarifasterminals WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasTerminal>(sql);
            return result;
        }
    }
    public async Task<TarifasTerminal> GetByContTypeAsync(string cont)
    {
        var sql = $"SELECT * FROM tarifasterminals WHERE contype='{cont}'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasTerminal>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(TarifasTerminal entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE tarifasterminals SET 
                    description = @description, 
                    contype = @contype, 
                    gastofijo = @gastofijo, 
                    gastovariable = @gastovariable
                             WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }

    public async Task<int> UpdateByDepoContTypeAsync(TarifasTerminal entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE tarifasterminals SET 
                    description = @description, 
                    contype = @contype, 
                    gastofijo = @gastofijo, 
                    gastovariable = @gastovariable
                             WHERE contype=@contype";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}