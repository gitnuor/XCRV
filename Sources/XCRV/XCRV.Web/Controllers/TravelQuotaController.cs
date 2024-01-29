using Dapper;
using Dapper.Oracle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.OracleInfrastructure.Repositories;

namespace XCRV.Web.Controllers
{
    public class TravelQuotaController : BaseController
    {
        private readonly ILogger<TravelQuotaController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly string _connectionStringIRIS;
        private readonly string _connectionStringCredit;
        public TravelQuotaController(ILogger<TravelQuotaController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            this._connectionStringIRIS = _configuration.GetConnectionString(DatabaseConnection.DbConnectionStringIRIS);
            this._connectionStringCredit = _configuration.GetConnectionString(DatabaseConnection.DbConnectionStringCredit);
        }

        [Filters.AuthorizeActionFilter]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchEndorsementDetail(string searchPassport)
        {
            string extensiveType = string.Empty;
            string _pbsFlag = "1";

            string message = "Sorry!!! No Data Found!!!";

            PassportEndorsementDebit data1 = new PassportEndorsementDebit();
            PassportEndorsementDebit data2 = new PassportEndorsementDebit();
            PassportEndorsementDebit data3 = new PassportEndorsementDebit();

            var claims = User.Claims;
            string isStatementTrue = claims.FirstOrDefault(p => p.Type.Equals("IsStatementTrue")).Value.ToString();
            string userName = claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            data1 = await getPassportEndorseDetailsDebit(searchPassport);
            data2= await getPassportEndorseDetailsCredit(searchPassport);
            //if (data1.qlimit_assigned == null || data1.qusagepercentageamount == null || data1.qlimit_amount == null || data1.PassportNo==null)
            if(data1 == null)
            {
                data3.qlimit_assigned = "0";
                data3.qusagepercentageamount = "0";
                data3.qlimit_amount = "0";
                data3.PassportNo = "";
                data3.limit_type = "";
            }
            else {
                data3.qlimit_assigned = data1.qlimit_assigned;
                data3.qusagepercentageamount = data1.qusagepercentageamount;
                data3.qlimit_amount = data1.qlimit_amount;
                data3.customerid = data1.customerid;
                data3.ParamValue = data1.ParamValue;
                data3.limit_type = "Travel Quota";

            }       
            if (data2 == null)
            {

                data3.qlimit_assigned_credit = "0";
                data3.qusagepercentageamount_credit = "0"; 
                data3.qlimit_amount_credit = "0";
                data3.limit_type_cr = "";
            }
            else {
                data3.qlimit_assigned_credit = data2.qlimit_assigned_credit;
                data3.qusagepercentageamount_credit = data2.qusagepercentageamount_credit;
                data3.qlimit_amount_credit = data2.qlimit_amount_credit;
                data3.cb_idno = data2.cb_idno;
                data3.cb_passport_no = data2.cb_passport_no;
                data3.limit_type_cr = "Travel Quota";
            }


            return Json(new { data = data3, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });

        }

        public async Task<PassportEndorsementDebit> getPassportEndorseDetailsDebit(string searchPassport)
        {
            var sql = string.Empty;
            var sql2= string.Empty;
            sql = String.Format(@"select tmq.customerid,cust.ParamValue 
                           ,sum(tmq.qlimit_assigned/100) qlimit_assigned 
                          ,sum(tmq.qusagepercentageamount/100) qusagepercentageamount
                          ,sum(tmq.qlimit_amount/100) qlimit_amount
                          from IRIS_CUSTOM.tbltmqcustomerlimit TMQ inner join iris_cms.tblcustomerdetail cust on cust.customerid =tmq.customerid
                          and cust.paramid in ('131') and cust.paramvalue ='{0}' group by tmq.customerid,cust.ParamValue", searchPassport);

             sql2= String.Format(@"select tmq.customerid,cust.ParamValue 
                           ,sum(tmq.qlimit_assigned/100) qlimit_assigned 
                          ,sum(tmq.qusagepercentageamount/100) qusagepercentageamount
                          ,sum(tmq.qlimit_amount/100) qlimit_amount
                          from IRIS_CUSTOM.tbltmqcustomerlimit TMQ inner join iris_cms.tblcustomerdetail cust on cust.customerid =tmq.customerid
                          and cust.paramid in ('132') and cust.paramvalue ='{0}' group by tmq.customerid,cust.ParamValue", searchPassport);
            

            var parameters = new DynamicParameters();
            try
            {
                
                using (var connection = new OracleConnection(_connectionStringIRIS))
                {
                    connection.Open();
                    var result = (await connection.QueryAsync<PassportEndorsementDebit>(sql, parameters, commandType: CommandType.Text)).FirstOrDefault();

                    if (result == null)
                    { 
                      var result2= (await connection.QueryAsync<PassportEndorsementDebit>(sql2, parameters, commandType: CommandType.Text)).FirstOrDefault();
                      connection.Close();
                      return result2;
                    }
                    connection.Close();
                    return result;
                }

            }
            catch (Exception ex)
            {
                var test = ex.Message;
                throw;
            }
        }

        public async Task<PassportEndorsementDebit> getPassportEndorseDetailsCredit(string searchPassport)
        {
            string sqlSrt = String.Format(@"SELECT   
           cb_idno,cb_passport_no,
           sum(nvl((tq_limit * tq_usages_rate) / 100, 0)) qlimit_assigned_credit,
           -- tq_billed,
            --unmtch_amt,
            sum( tq_billed + nvl(unmtch_amt, 0) ) qusagepercentageamount_credit,
            sum( nvl((tq_limit * tq_usages_rate) / 100, 0) - ( tq_billed + nvl(unmtch_amt, 0) ) ) qlimit_amount_credit
        FROM
            (
                SELECT
                    cb_cardholder_no,
                    cb_idno,
                    cb_status_cd,
                    cb_plastic_cd,
                    cb_line_limit,
                    cb_outstd_bal,
                    cb_last_age_cd,
                    cb_basic_supp_ind,
                    cb_passport_no,
                    cb_non_saar_usage_rate   tq_usages_rate,
                    cb_tq_checking,
                    cb_non_saar_limit        tq_limit,
                    cb_ytd_non_saar          tq_billed,
                    a.cb_individual_acctno
                FROM
                    cardpro.cp_crdtbl   d,
                    cardpro.cp_indacc   a,
                    cardpro.cp_fintbl   b,
                    cardpro.cp_csttbl   c
                WHERE
                    cb_cardholder_no = cb_ind_cardholder_no
                    AND a.cb_fin_acctno = b.cb_fin_acctno
                    AND c.cb_customer_idno = d.cb_idno
            ) a
            LEFT JOIN (
                SELECT
                    tqs_individual_acctno,
                    SUM(tqs_unmtch_amt) unmtch_amt
                FROM
                    cp_apvtqs
                WHERE 
                    tqs_appv_date BETWEEN to_char(to_date(sysdate - 9), 'yyyymmdd') AND to_char(to_date(sysdate), 'yyyymmdd')
                GROUP BY
                    tqs_individual_acctno
            ) b ON a.cb_individual_acctno = b.tqs_individual_acctno
   WHERE  
    cb_passport_no = '{0}' group by cb_idno,cb_passport_no", searchPassport);
   // group by cb_idno ;);


            var sql = sqlSrt; //DatabasePackage.FINACAL_PACKAGE_NAME + DatabaseProcedure.FinacalProcedure.SP_GET_SOL;

            var parameters = new DynamicParameters();
            try
            {

                using (var connection = new OracleConnection(_connectionStringCredit))
                {
                    connection.Open();
                    //parameters.Add("CUR_CUSTOMER", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    //parameters.Add("P_IsStatementTrue", isStatementTrue);
                    var result = (await connection.QueryAsync<PassportEndorsementDebit>(sql, parameters, commandType: CommandType.Text)).FirstOrDefault();
                    connection.Close();

                    return result;
                }

            }
            catch (Exception ex)
            {
                var test = ex.Message;
                throw;
            }
        }



    }
}
