namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;

public class ProductRepositoryMemory : IProductRepository
{
    private static List<Product> store = new List<Product>();
    public async Task<Product> GetByIdAsync(int id){
        return store[id];
    }
    public async Task<IEnumerable<Product>> GetAllAsync(){
        return store;
    }
    public async Task<int> AddAsync(Product entity){
        var id = store.Count();
        entity.Id = id++;
        store.Add(entity);

        return entity.Id;
    }
    public async Task<int> UpdateAsync(Product entity){
        store[entity.Id] = entity;
        
        return entity.Id;
    }
    public async Task<int> DeleteAsync(int id){
        store.RemoveAt(id);
        
        return id;
    }
}