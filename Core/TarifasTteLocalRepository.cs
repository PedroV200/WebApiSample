namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class TarifasTteLocalRepository : ITarifasTteLocalRepository
{
 private readonly IConfiguration configuration;
    public TarifasTteLocalRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(TarifasTteLocal entity)
    {
        var sql = $"INSERT INTO tarifasttelocal (contype, fleteint, devacio, demora, guarderia, totgastos) VALUES ('{entity.contype}','{entity.fleteint.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.devacio.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.demora.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.guarderia.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.totgastos.ToString(CultureInfo.CreateSpecificCulture("en-US"))}')";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM tarifasttelocal WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<IEnumerable<TarifasTteLocal>> GetAllAsync()
    {
        var sql = "SELECT * FROM tarifasttelocal";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<TarifasTteLocal>(sql);
        }
    }
    public async Task<TarifasTteLocal> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM tarifasttelocal WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasTteLocal>(sql);
            return result;
        }
    }
    public async Task<TarifasTteLocal> GetTarifaTteByContAsync(string cont)
    {
        var sql = $"SELECT * FROM tarifasttelocal WHERE contype='{cont}'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<TarifasTteLocal>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(TarifasTteLocal entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE tarifasttelocal SET 
                    contype = @contype, 
                    fleteint = @fleteint, 
                    devacio = @devacio, 
                    demora = @demora,
                    guarderia = @guarderia,
                    totgastos = @totgastos
                             WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }

     public async Task<int> UpdateTteTarifaByContTypeAsync(TarifasTteLocal entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";

        // QUERY anterior no funcionaba
        //var sql = @"UPDATE tarifasdepositos SET depo = @depo, contype = @contype, descarga = @descarga, ingreso = @ingreso, totingreso = @totingreso, carga = @carga,armado = @armado, egreso = @egreso, totegreso = @totegreso WHERE depo = @depo AND contype=@contype";
        
        var sql = @"UPDATE tarifasttelocal SET 
                    contype = @contype, 
                    fleteint = @fleteint, 
                    devacio = @devacio, 
                    demora = @demora,
                    guarderia = @guarderia,
                    totgastos = @totogastos,
                WHERE contype = @contype";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}