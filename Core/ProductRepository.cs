namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;

public class ProductRepository : IProductRepository
{
    private readonly IConfiguration configuration;
    public ProductRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(Product entity)
    {
        entity.AddedOn = DateTime.Now;
        var sql = $"INSERT INTO Products (Name, Description, Barcode, Rate) VALUES ('{entity.Name}','{entity.Description}','{entity.Barcode}',{entity.Rate})";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM Products WHERE Id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var sql = "SELECT * FROM Products";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<Product>(sql);
        }
    }
    public async Task<Product> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM Products WHERE Id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Product>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(Product entity)
    {
        //entity.ModifiedOn=DateTime.Now;
        //entity.ModifiedOn=DateTime.Now;
        //var sql = $"UPDATE Products SET Name = '{entity.Name}', Description = '{entity.Description}', Barcode = '{entity.Barcode}', Rate = {entity.Rate}, ModifiedOn = {entity.ModifiedOn}, AddedOn = {entity.AddedOn}  WHERE Id = {entity.Id}";
        var sql = @"UPDATE Products SET 
                    Name = @Name, 
                    Description = @Description, 
                    Barcode = @Barcode, 
                    Rate = @Rate, 
                    AddedOn = @AddedOn,
                    ModifiedOn = @ModifiedOn 
                             WHERE Id = @Id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}