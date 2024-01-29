using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XCRV.Domain.Entities;

namespace XCRV.Application.Interfaces
{
    public interface ICustomerMemoRepository
    {
        Task<IReadOnlyList<CustomerMemo>> GetCustomerMemosByCif(string cif);
        Task<int> InsertCustomerMemo(string MemoText, string UserID, string CustomerID);
        Task<string> InsertXCRVLog(string LogType, string Method, string RequestPath, string RequestString, string StatusCode,
            string ElapsedTime, string UserId, string UserName, string ProjectId, string ExceptionDetails);
        Task<int> UpdateCustomerMemo(long ID, string MemoText, string UserID);
    }
}
