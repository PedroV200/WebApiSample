namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization; 

public class EstimateDetailDBRepository : IEstimateDetailDBRepository
{
    private readonly IConfiguration configuration;
    public EstimateDetailDBRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(EstimateDetailDB entity)
    {
        var sql = $"INSERT INTO estimatedetails (Modelo, Ncm, PesoUnitxCaja, CbmxCaja, PcsxCaja, FobUnit, CantPcs, IdEstHeader) VALUES ('{entity.Modelo}','{entity.Ncm}','{entity.PesoUnitxCaja.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.CbmxCaja.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',{entity.PcsxCaja},'{entity.FobUnit.ToString(CultureInfo.CreateSpecificCulture("en-US"))}',{entity.CantPcs},{entity.IdEstHeader})";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int Id)
    {
        var sql = $"DELETE FROM estimatedetails WHERE Id = {Id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<int> DeleteByIdEstHeaderAsync(int IdEstHeader)
    {
        var sql = $"DELETE FROM estimatedetails WHERE IdEstHeader = {IdEstHeader}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }
    public async Task<IEnumerable<EstimateDetailDB>>GetAllAsync()
    {
        var sql = "SELECT * FROM estimatedetails";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<EstimateDetailDB>(sql);
        }
    }
        public async Task<IEnumerable<EstimateDetailDB>>GetAllByIdEstHeadersync(int Id)
    {
        var sql = $"SELECT * FROM estimatedetails WHERE IdEstHeader={Id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<EstimateDetailDB>(sql);
        }
    }

    public async Task<EstimateDetailDB> GetByIdAsync(int Id)
    {
        var sql = $"SELECT * FROM estimatedetails WHERE Id = {Id}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<EstimateDetailDB>(sql);
            return result;
        }
    }
    public async Task<int> UpdateAsync(EstimateDetailDB entity)
    {
        
        var sql = @"UPDATE estimatedetails SET 
                    modelo = @modelo, 
                    ncm = @ncm, 
                    pesounitxcaja = @pesounitxcaja, 
                    cbmxcaja = @cbmxcaja, 
                    pcsxcaja = @pcsxcaja,
                    fobunit = @fobunit,
                    cantpcs = @cantpcs,
                    IdEstHeader = @IdEstHeader
                             WHERE id = @id";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}