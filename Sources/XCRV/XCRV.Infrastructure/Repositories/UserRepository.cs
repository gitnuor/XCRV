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
    internal class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private string probasiFlag = string.Empty;
        public UserRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString(DatabaseConnection.SqlCSPMConnection);
        }

        public async Task<IReadOnlyList<Users>> GetUserInfoByUserName(string userName)
        {
            var sql = DatabaseProcedure.CspmProcedure.SP_UserInfo_forXCRV360V2;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@strUserName", userName);
                var result = await connection.QueryAsync<Users>(sql, parameters, commandType: CommandType.StoredProcedure);
                connection.Close();
                return result.ToList();
            }
        }

        public async Task<IReadOnlyList<Users>> GetXcrvUserName()
        {
            var sql = "Select distinct UserName from users";
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                var result = await connection.QueryAsync<Users>(sql, commandType: CommandType.Text);
                connection.Close();
                return result.OrderBy(x=>x.UserName).ToList();
            }
        }

        public async Task<IReadOnlyList<Users>> GetProbasiByUser(string userID)
        {
            probasiFlag = _configuration.GetSection("AppSettings").GetSection("PrabasiFlag").Value;
            try
            {
                // var sql = "select  IsNull(GroupName, '') GroupName from UserGroup g inner join UsersGroupAssign s on g.usergroupid = s.usergroupid inner join users u on u.usersid = s.usergroupid where u.usersid =@userID and g.GroupName ='Probashi Account'";
                var sql = "select GroupName from UsersGroupAssign ua inner join UserGroup g on g.UserGroupId = ua.UserGroupId where ua.UserId =@userID and g.GroupName ='" + probasiFlag + "'";
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@userID", Convert.ToString(userID));
                    var result = await connection.QueryAsync<Users>(sql, parameters, commandType: CommandType.Text);
                    
                    connection.Close();
                    //  if(result.Count==0)
                    // return string.Compare(result.FirstOrDefault().GroupName) ? "" : result.FirstOrDefault().GroupName;
                    return result.ToList();
                    

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        public async Task<string> GetProjectName(string projectId)
        {
            var sql = "Select SoftwareTitle from Projects where ProjectsId = @projectId";
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@projectId", projectId);
                var result = await connection.QueryAsync(sql, parameters, commandType: CommandType.Text);
                connection.Close();
                return result.FirstOrDefault().SoftwareTitle;
            }
        }
    }
}
