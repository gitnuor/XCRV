using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface ICrvCardChqRepository 
    {
        Task<ChqCustomer> GetChqCustInfoList(string accountNo);
        Task<IList<ChqStatus>> GetChqstatusByRange(string accountNo, string leafFrom, string leafTo, string user_id);
        Task<string> ChangeChqstatusByRange(ChqStopRange chqStopRange);
        Task<IList<ChqReqLog>> GetChqLog(string accountNo, string user_id);
        Task<IList<ChqReqLog>> GetChqReqLog(string accountNo, string frmDate, string toDate, string makerUserID, string checkerUserID);
        Task<string> VerifyCheque(IList<ChqStopRangeVerify> chqStopRangeList, string ChekerUserID);

        Task<IList<ChqStopReport>> GetChqStopReport(string accountNo, string chqno, string frmDate, string toDate, string user_id);
        Task<IList<ChqStopSummaryReport>> GetChqStopSummaryReport(string frmDate, string toDate);
        Task<IList<ChqStopReport>> GetMakerUserList();
    }
}
