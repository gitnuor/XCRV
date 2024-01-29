using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;

namespace XCRV.OracleInfrastructure.Repositories
{
    public class QmsCardProRepository : IQmsCardProRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public QmsCardProRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.DbConnectionStringOracleCardpro);
        }

        public async Task<CardCustomerInformation> GetCustomerInformationByRefNo(string refNo)
        {
            var sql = DatabasePackage.QMS_CARDPRO_PACKAGE_NAME + DatabaseProcedure.QmsCardProProcedure.Card_Chq_Cust_Info;
            var parameters = new OracleDynamicParameters();

            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                parameters.Add("v_gdetails", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                parameters.Add("CusIDNo", refNo);
                var result = (await connection.QueryAsync<CardCustomerInformation>(sql, parameters, commandType: CommandType.StoredProcedure));
                connection.Close();

                return result.FirstOrDefault();
            }
        }
    }
}
