namespace WebApiSample.Core;

using WebApiSample.Infrastructure;
using WebApiSample.Models;
using Npgsql;
using Dapper;
using System.Data;
using System.Globalization;


// ADVERTENCIA: El PK de la base de datos es la posicion arancelaria, que no es
// un valor umerico, son numeros separados por puntos como si fuera una IP
// para poder honrar la Interface que require que tanto el metodo GET(by ID) como 
// el metodo DELETE usaran el id automatico agregado por la DB y no la posicion
// arancelaria en si misma.
// Solo UPDATE que recibe el entity completo puede hacer x posicion arancelaria (CODE)

public class NCMrepository : INCMrepository
{
private readonly IConfiguration configuration;
    public NCMrepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<int> AddAsync(NCM entity)
    {
        var sql = $"INSERT INTO ncm (code, description, anexo, die, te, iva, iva_ad, imp_int, licensia, bit_bk, vc, peso_valor) VALUES ('{entity.code}','{entity.description}','{entity.anexo}','{entity.die.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.te.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.iva.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.iva_ad.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.imp_int.ToString(CultureInfo.CreateSpecificCulture("en-US"))}','{entity.licensia}', '{entity.bit_bk}','{entity.vc}','{entity.peso_valor.ToString(CultureInfo.CreateSpecificCulture("en-US"))}')";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
    public async Task<int> DeleteAsync(int code)
    {
        var sql = $"DELETE FROM ncm WHERE id = {code}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<int> DeleteByStrAsync(string code)
    {
        var sql = $"DELETE FROM ncm WHERE code = '{code}'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            var result = await connection.ExecuteAsync(sql);

            return result;
        }
    }

    public async Task<IEnumerable<NCM>> GetAllAsync()
    {
        var sql = "SELECT * FROM ncm";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();

            return await connection.QueryAsync<NCM>(sql);
        }
    }
    public async Task<NCM> GetByIdAsync(int code)
    {
        var sql = $"SELECT * FROM ncm WHERE id = {code}";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<NCM>(sql);
            return result;
        }
    }

        public async Task<NCM> GetByIdStrAsync(string code)
    {
        var sql = $"SELECT * FROM ncm WHERE code = '{code}'";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<NCM>(sql);
            return result;
        }
    }

    public async Task<int> UpdateAsync(NCM entity)
    {
        
        var sql = @"UPDATE ncm SET 
                    code = @code,   
                    description = @description, 
                    anexo = @anexo,
                    die = @die, 
                    te = @te, 
                    iva = @iva,
                    iva_ad = @iva_ad, 
                    imp_int = @imp_int, 
                    licensia = @licensia,
                    bit_bk = @bit_bk,
                    vc = @vc,
                    peso_valor = @peso_valor

                             WHERE code = @code";
        using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var result = await connection.ExecuteAsync(sql, entity);
            return result;
        }
    }
}
