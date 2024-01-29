using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XCRV.Web
{
    public class AccountNumberValidationHelper
    {
        private static string _accountNumberReg = @"^([0-9]{13}|[0-9]{16})$";

        public static bool IsAccountNoValid(string accountNumber)
        {
            return Regex.IsMatch(accountNumber, _accountNumberReg);
        }
    }
}
