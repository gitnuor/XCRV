using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XCRV.Web.Controllers
{
    public class ErrorController : Controller
    {
        
        public IActionResult UnauthorizedPage()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult BrowserIssue()
        {
            return View();
        }

    }
}
