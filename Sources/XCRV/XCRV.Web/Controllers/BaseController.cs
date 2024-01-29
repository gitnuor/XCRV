using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Web.Common;

namespace XCRV.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public AJaxResponse CommonAjaxResponse(string messageType, string message, string responseCode)
        {
            using (var response = new AJaxResponse())
            {
                response._AjaxCode = responseCode;
                response._DisplayMessage = message;
                response._DisplayMessageType = messageType.ToString();
                return response;
            }
        }


        public bool IsNumberString(string str)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str.Trim(), @"^\d+$");
        }
    }
}
