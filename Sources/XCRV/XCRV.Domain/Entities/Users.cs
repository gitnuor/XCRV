using System;
using System.Collections.Generic;
using System.Text;

namespace XCRV.Domain.Entities
{
    public class Users
    {
        public int UsersId { get; set; }

        public string UserName { get; set; }

        public string Passwd { get; set; }

        public int UserGroupId { get; set; }

        public int DeptId { get; set; }

        public string Designation { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PinNumber { get; set; }

        public string MailAddress { get; set; }

        public string Status { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string IsStatementTrue { get; set; }

        public bool IsNewUser { get; set; }

        public DateTime LastPasswordChangedDate { get; set; }

        public int FailedPasswordAttemptCount { get; set; }
        public string GroupName { get; set; }

    }

}
