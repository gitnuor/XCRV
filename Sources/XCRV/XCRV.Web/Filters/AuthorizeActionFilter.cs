using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Web.Models;

namespace XCRV.Web.Filters
{
    public class AuthorizeActionFilter : Microsoft.AspNetCore.Authorization.AuthorizeAttribute, IAuthorizationFilter
    {
        IEnumerable<MenuViewModel> PermissionList;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            PermissionList = context.HttpContext.Session.Get<IEnumerable<MenuViewModel>>("PermissionList");

            var action = context.RouteData.Values["action"];
            var controller = context.RouteData.Values["controller"];

            string url = "/"+ controller + "/" + action;

            bool isAuthorized = CheckUserPermission(url);
            if (!isAuthorized)
            {
                context.Result = new UnauthorizedResult();
            }
        }

        private bool CheckUserPermission(string action)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                return true;
            }
            else
            {
                return PermissionList.Where(p => p.MenuUrl.ToUpper().Equals(action.ToUpper())).Count() > 0;
            }
        }

        
    }
}
