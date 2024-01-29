using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.OracleInfrastructure.Repositories;

namespace XCRV.Infrastructure.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public MenuRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.SqlCRMConnection);
        }      

        public async Task<IReadOnlyList<Menu>> GetAllAsync()
        {
            var sql = DatabaseProcedure.CspmProcedure.SP_GetXCRV360V2Menu;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var result = await connection.QueryAsync<Menu>(sql, commandType: CommandType.StoredProcedure);
                connection.Close();
                return result.ToList();
            }
        }




    }
}
