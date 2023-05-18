namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class SeguroRepository : ISeguroRepository
{
    private readonly IConfiguration configuration;
    public SeguroRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(Seguro entity)
    {
        var sql = $"INSERT INTO seguros (description, prima, demora, impInterno, sellos ) VALUES ('{entity.description}'," +
            $"'{entity.prima.ToString(CultureInfo.CreateSpecificCulture("en-US"))}'," + // metodo tostring.(CultureInfo()), permite cargar a sql num con .float
            $"'{entity.demora.ToString(CultureInfo.CreateSpecificCulture("en-US"))}'," +
            $"'{entity.impInterno.ToString(CultureInfo.CreateSpecificCulture("en-US"))}'," +
            $"'{entity.sellos.ToString(CultureInfo.CreateSpecificCulture("en-US"))}')";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM seguros WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<Seguro>> GetAllAsync()
    {
        var sql = "SELECT * FROM seguros";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<Seguro>(sql);
        }
    }
    public async Task<Seguro> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM seguros WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Seguro>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(Seguro entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE seguros SET
                           description = @description,
                           prima = @prima,
                           demora = @demora,
                           impInterno = @impInterno,
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