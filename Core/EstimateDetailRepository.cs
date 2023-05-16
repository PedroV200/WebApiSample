namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization; 

public class EstimateDetailRepository : IEstimateDetailRepository
{
    private readonly IConfiguration configuration;
    public EstimateDetailRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(EstimateDetail entity)
    {
        var sql = $"INSERT INTO p_estimatedetails (modelo, ncm, pesounitxcaja, cbmxcaja, pcsxcaja, fobunit, cantpcs, code) VALUES ('{entity.modelo}','{entity.ncm}','{entity.pesounitxcaja.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.cbmxcaja.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',{entity.pcsxcaja},'{entity.fobunit.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',{entity.cantpcs},{entity.code})";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM p_estimatedetails WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<EstimateDetail>>GetAllAsync()
    {
        var sql = "SELECT * FROM p_estimatedetails";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<EstimateDetail>(sql);
        }
    }
        public async Task<IEnumerable<EstimateDetail>>GetAllByCodeAsync(int code)
    {
        var sql = $"SELECT * FROM p_estimatedetails WHERE code={code}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<EstimateDetail>(sql);
        }
    }

    public async Task<EstimateDetail> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM p_estimatedetails WHERE id = {id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<EstimateDetail>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(EstimateDetail entity)
    {
        
        var sql = @"UPDATE p_estimatedetails SET 
                    modelo = @modelo, 
                    ncm = @ncm, 
                    pesounitxcaja = @pesounitxcaja, 
                    cbmxcaja = @cbmxcaja, 
                    pcsxcaja = @pcsxcaja,
                    fobunit = @fobunit,
                    cantpcs = @cantpcs,
                    code = @code
                             WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}