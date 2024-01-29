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
    public class MenuPrevilegeRepository : IMenuPrevilegeRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public MenuPrevilegeRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.SqlCSPMConnection);
        }

        public async Task<IReadOnlyList<MenuPrevilege>> GetByUserIdAndGroupIdAndProjectId(int userId, int groupId, int projectId)
        {
            var sql = DatabaseProcedure.CspmProcedure.spGetMenuPrevilegeXCRV;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@ProjID", projectId);
                parameters.Add("@UserID", userId);
                parameters.Add("@GroupID", groupId);

                var result = await connection.QueryAsync<MenuPrevilege>(sql, parameters, commandType: CommandType.StoredProcedure);
                connection.Close();
                return result.ToList();
            }
        }
    }
}
