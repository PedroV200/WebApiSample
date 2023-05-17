namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class TarifasDepositoRepository : ITarifasDepositoRepository
{
 private readonly IConfiguration configuration;
    public TarifasDepositoRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(TarifasDeposito entity)
    {
        var sql = $"INSERT INTO tarifasdepositos (depo, contype, descarga, ingreso, totingreso, carga, armado, egreso, totegreso) VALUES ('{entity.depo}','{entity.contype}','{entity.descarga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.ingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.totingreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.carga.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.armado.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.egreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.totegreso.ToString(CultureInfo.CreateSpecificCulture("en-US"))}')";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM tarifasdepositos WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<int> DeleteByDepoContTypeAsync(string dep,string cont)
    {
        var sql = $"DELETE FROM tarifasdepositos WHERE depo = '{dep}' AND contype = '{cont}'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<TarifasDeposito>> GetAllAsync()
    {
        var sql = "SELECT * FROM tarifasdepositos";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasDeposito>(sql);
        }
    }
    public async Task<TarifasDeposito> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM tarifasdepositos WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasDeposito>(sql);
            return result;
        }
    }
    public async Task<TarifasDeposito> GetByDepoContTypeAsync(string depo, string cont)
    {
        var sql = $"SELECT * FROM tarifasdepositos WHERE depo = '{depo}' AND contype='{cont}'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasDeposito>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(TarifasDeposito entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE tarifasdepositos SET 
                    depo = @depo, 
                    contype = @contype, 
                    descarga = @descarga, 
                    ingreso = @ingreso, 
                    totingreso = @totingreso,
                    carga = @carga,
                    armado = @armado,
                    egreso = @egreso,
                    totegreso = @totegreso
                             WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }

     public async Task<int> UpdateByDepoContTypeAsync(TarifasDeposito entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE tarifasdepositos SET 
                    depo = @depo, 
                    contype = @contype, 
                    descarga = @descarga, 
                    ingreso = @ingreso, 
                    totingreso = @totingreso,
                    carga = @carga,
                    armado = @armado,
                    egreso = @egreso,
                    totegreso = @totegreso
                             WHERE depo = @depo AND contype=@contype";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}