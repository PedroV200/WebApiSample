namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;

public class ContenedorRepository : IContenedorRepository
{
 private readonly IConfiguration configuration;
    public ContenedorRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(Contenedor entity)
    {
        var sql = $"INSERT INTO contenedores (description, weight, volume) VALUES ('{entity.description}','{entity.weight.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.volume.ToString(CultureInfo.CreateSpecificCulture("en-US"))}')";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM contenedores WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

        public async Task<int> DeleteByTipoContAsync(string type)
    {
        var sql = $"DELETE FROM contenedores WHERE description = '{type}'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<Contenedor>> GetAllAsync()
    {
        var sql = "SELECT * FROM contenedores";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<Contenedor>(sql);
        }
    }
    public async Task<Contenedor> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM contenedores WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Contenedor>(sql);
            return result;
        }
    }
        public async Task<Contenedor> GetByTipoContAsync(string type)
    {
        var sql = $"SELECT * FROM contenedores WHERE description = '{type}'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Contenedor>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(Contenedor entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE contenedores SET 
                    description = @description, 
                    weight = @weight, 
                    volume = @volume
                             WHERE description = @description";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}