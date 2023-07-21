namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class TarifasFwdContRepository : ITarifasFwdContRepository
{
 private readonly IConfiguration configuration;
    public TarifasFwdContRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(TarifasFwdCont entity)
    {
        var sql = $"INSERT INTO tarifasfwds (contype, fwdfrom, costoflete, costoflete040, costoflete060, gastos1, gastos2) VALUES ('{entity.contype}','{entity.fwdfrom}','{entity.costoflete.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.costoflete040.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.costoflete060.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.gastos1.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.gastos2.ToString(CultureInfo.CreateSpecificCulture("en-US"))}')";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM tarifasfwds WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<int> DeleteByFwdContTypeAsync(string fwd,string cont)
    {
        var sql = $"DELETE FROM tarifasfwds WHERE fwdfrom = '{fwd}' AND contype = '{cont}'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<TarifasFwdCont>> GetAllAsync()
    {
        var sql = "SELECT * FROM tarifasfwds";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasFwdCont>(sql);
        }
    }
    public async Task<TarifasFwdCont> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM tarifasfwds WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasFwdCont>(sql);
            return result;
        }
    }
    public async Task<TarifasFwdCont> GetByFwdContTypeAsync(string fwd, string cont)
    {
        var sql = $"SELECT * FROM tarifasfwds WHERE fwdfrom = '{fwd}' AND contype='{cont}'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasFwdCont>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(TarifasFwdCont entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE tarifasfwds SET 
                    contype = @contype, 
                    fwdfrom = @fwdfrom, 
                    costoflete = @costoflete, 
                    costoflete040 = @costoflete040, 
                    costoflete060 = @costoflete060,
                    gastos1 = @gastos1,
                    gastos2 = @gastos2
                             WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }

    

     public async Task<int> UpdateByFwdContTypeAsync(TarifasFwdCont entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE tarifasfwds SET 
                    contype = @contype, 
                    fwdfrom = @fwdfrom, 
                    costoflete = @costoflete, 
                    costoflete040 = @costoflete040, 
                    costoflete060 = @costoflete060,
                    gastos1 = @gastos1,
                    gastos2 = @gastos2
                             WHERE contype = @contype AND fwdfrom = @fwdfrom";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }

    public async Task<IEnumerable<string>>GetOriginCountry()
    {
        var sql = "select distinct fwdfrom from tarifasfwds";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<string>(sql);
        }
    }
}