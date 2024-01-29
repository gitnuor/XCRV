using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface ICardChqRepository
    {
        Task<ChqLeafLimit> GetCardChqLeafSearchLimit(string CHQ_Type);
        Task<IList<ChqBookInfo>> GetUpdatedChqBookData(string refNo, string agentID, string supervisorID, string subOrdinateID, string cardno,
            string chqno, string fromdate, string todate, string remarks, string intQueryType);
        Task<IList<CardChqActiveVerify>> GetChqBookDataForVerify(string refNo, string agentID, string supervisorID, string subOrdinateID, string cardno,
            string chqno, string fromdate, string todate, string remarks);
        Task<bool> VerifyChqBookData(string refNo, string agentID, string supervisorID, string subOrdinateID, string cardno,
            string chqno, string fromdate, string todate, string remarks);
        Task<bool> ApproveChqBookData(string refNo, string agentID, string supervisorID, string subOrdinateID, string cardno,
            string chqno, string fromdate, string todate, string remarks);
        Task<IList<CardChqEntity>> GetIssuedChqBookData(string refNo, string chqBookSL, string remarks, string chqStatus, string userID, string queryType);

        Task<long> ChequeBookCancelEntry(CardDeactiveEntity entity);
        Task<bool> ChequeBookCancelEntry(IList<CardDeactiveEntity> entityList);

        Task<IList<CardChequeDeactiveVerifyEntity>> GetPendingCancelChequeBookData(string checkerId , string fromDate, string toDate);
        Task<IList<CardChequeDeactiveVerifyEntity>> CancelVerifyCheck(string numChqBookSL, string checkerId);
        Task<bool> CancelVerifyEntry(CardChequeDeactiveVerifyEntity entity);
        Task<IList<CardChqReport>> CardChqActiveDeactiveReport(string cardNumber, string chqueNumber, string fromDate, string toDate, string userID, string reportType);
        Task<IList<CardChequeDeactiveVerifyEntity>> GetUserList();

    }
}
