namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class CnstRepository : ICnstRepository
{
 private readonly IConfiguration configuration;
    public CnstRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(Cnst entity)
    {
        try
        {
            var sql = $"INSERT INTO constantes (description,val,detalle) VALUES ('{entity.description}','{entity.val.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.detalle}')";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }
        catch (Exception)
        {
            return -1;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        try
        {
        var sql = $"DELETE FROM constantes WHERE id = {id}";
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
    public async Task<IEnumerable<Cnst>> GetAllAsync()
    {
        try
        {
            var sql = "SELECT * FROM constantes";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                return await connection.QueryAsync<Cnst>(sql);
            }
        }catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<Cnst> GetByIdAsync(int id)
    {
        try
        {
            var sql = $"SELECT * FROM constantes WHERE id = {id}";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<Cnst>(sql);
                return result;
            }
        }
        catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }
    public async Task<int> UpdateAsync(Cnst entity)
    {
        try
        {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql =  @"UPDATE constantes SET 
                    description = @description, 
                    val = @val, 
                    detalle = @detalle 
                WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
        }catch(Exception)
        {
            return -1;
        }
    }
}