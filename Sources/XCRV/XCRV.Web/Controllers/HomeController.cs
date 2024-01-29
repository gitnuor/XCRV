using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Wangkanai.Detection.Services;
using XCRV.Application.Interfaces;
using XCRV.Web.Helpers;
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IConfiguration _configuration;

        private string redirectUrl = string.Empty;

        private readonly IDetectionService _detectionService;

        #region PageMethod

        public string GetWebBrowserName()
        {
            //var userAgent = HttpContext.Request.Headers["User-Agent"];

            return _detectionService.Browser.Name.ToString() ;

        }

        private bool IsLocalUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            Uri absoluteUri;
            if (Uri.TryCreate(url, UriKind.Absolute, out absoluteUri))
            {
                return String.Equals(HttpContext.Request.Host.Value, absoluteUri.Host,
                            StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                bool isLocal = !url.StartsWith("http:", StringComparison.OrdinalIgnoreCase)
                    && !url.StartsWith("https:", StringComparison.OrdinalIgnoreCase)
                    && Uri.IsWellFormedUriString(url, UriKind.Relative);
                return isLocal;
            }
        }

        private async Task GetUserMenuList(string USERID, string PROJECTSID, string USERGROUP)
        {
            int userId = 0;
            int groupId = 0;
            int projectId = 0;

            var claims = HttpContext.User.Claims;

            int.TryParse(USERID, out userId);
            int.TryParse(PROJECTSID, out projectId);
            int.TryParse(USERGROUP, out groupId);

            var allMenu = await _unitOfWork.MenuRepo.GetAllAsync();


            Helpers.MenuMappingList menuMappingList = new Helpers.MenuMappingList();



            var menuPrivilege = await _unitOfWork.MenuPrevilegeRepo.GetByUserIdAndGroupIdAndProjectId(userId, groupId, projectId);

            IList<MenuViewModel> menuList = new List<MenuViewModel>();

            IList<MenuViewModel> permissionList = new List<MenuViewModel>();

            IList<MenuViewModel> parent = (from p in allMenu
                                           where p.ParentID == 0
                                           select new MenuViewModel()
                                           {
                                               Id = p.MenuId,
                                               MenuName = p.MenuText,
                                               MenuUrl = p.NavigateUrl,
                                               SubMenu = new List<MenuViewModel>()
                                           }).ToList();

            foreach (var m in parent)
            {
                var SubMenu = (from p in allMenu
                               where p.ParentID == m.Id
                               select new MenuViewModel()
                               {
                                   Id = p.MenuId,
                                   MenuName = p.MenuText,
                                   MenuUrl = p.NavigateUrl,
                                   SubMenu = new List<MenuViewModel>()

                               }).ToList();

                foreach (var sm in SubMenu)
                {
                    var subSubMenu = (from p in allMenu
                                      where p.ParentID == sm.Id
                                      select new MenuViewModel()
                                      {
                                          Id = p.MenuId,
                                          MenuName = p.MenuText,
                                          MenuUrl = p.NavigateUrl,
                                          SubMenu = new List<MenuViewModel>()

                                      }).ToList();

                    if (subSubMenu.Count == 0)
                    {
                        var d = menuPrivilege.Where(p => (!string.IsNullOrEmpty(p.MainMenuLinkPage) && p.MainMenuLinkPage.Equals(sm.MenuUrl.Trim())) ||
                                                         (!string.IsNullOrEmpty(p.SubMenuLinkPage) && p.SubMenuLinkPage.Equals(sm.MenuUrl.Trim())) ||
                                                         (!string.IsNullOrEmpty(p.SubChildMenuLinkPage) && p.SubChildMenuLinkPage.Equals(sm.MenuUrl.Trim())));
                        if (d.Count() > 0)
                        {
                            //if (System.Diagnostics.Debugger.IsAttached)
                            {
                                var map = menuMappingList.MenuMappings.FirstOrDefault(p => p.Url.ToUpper().Trim().Equals(sm.MenuUrl.ToUpper().Trim()));
                                if (map != null)
                                {
                                    sm.MenuUrl = map.RewriteUrl;
                                }
                            }

                            if(!sm.MenuUrl.ToLower().Contains("aspx"))
                            {
                                permissionList.Add(sm);
                                m.SubMenu.Add(sm);
                            }
                            
                        }
                    }
                    else
                    {
                        foreach (var ssm in subSubMenu)
                        {
                            var d = menuPrivilege.Where(p => (!string.IsNullOrEmpty(p.MainMenuLinkPage) && p.MainMenuLinkPage.Equals(ssm.MenuUrl)) ||
                                                         (!string.IsNullOrEmpty(p.SubMenuLinkPage) && p.SubMenuLinkPage.Equals(ssm.MenuUrl)) ||
                                                         (!string.IsNullOrEmpty(p.SubChildMenuLinkPage) && p.SubChildMenuLinkPage.Equals(ssm.MenuUrl)));
                            if (d.Count() > 0)
                            {
                                //if (System.Diagnostics.Debugger.IsAttached)
                                {
                                    var map = menuMappingList.MenuMappings.FirstOrDefault(p => p.Url.ToUpper().Equals(ssm.MenuUrl.ToUpper()));
                                    if (map != null)
                                    {
                                        ssm.MenuUrl = map.RewriteUrl;
                                    }
                                }

                                if(!ssm.MenuUrl.ToLower().Contains("aspx"))
                                {
                                    permissionList.Add(ssm);
                                    sm.SubMenu.Add(ssm);
                                }
                                
                            }
                        }

                        if (sm.SubMenu.Count > 0)
                        {
                            //if (System.Diagnostics.Debugger.IsAttached)
                            {
                                var map = menuMappingList.MenuMappings.FirstOrDefault(p => p.Url.ToUpper().Equals(sm.MenuUrl.ToUpper()));
                                if (map != null)
                                {
                                    sm.MenuUrl = map.RewriteUrl;
                                }
                            }

                            if (!sm.MenuUrl.ToLower().Contains("aspx"))
                            {
                                permissionList.Add(sm);
                                m.SubMenu.Add(sm);
                            }
                            
                        }
                    }
                }

                if (m.SubMenu.Count > 0)
                {
                    //if (System.Diagnostics.Debugger.IsAttached)
                    {
                        var map = menuMappingList.MenuMappings.FirstOrDefault(p => p.Url.ToUpper().Equals(m.MenuUrl.ToUpper()));
                        if (map != null)
                        {
                            m.MenuUrl = map.RewriteUrl;
                        }
                    }
                    if (!m.MenuUrl.ToLower().Contains("aspx"))
                    {
                        permissionList.Add(m);
                        menuList.Add(m);
                    }
                }
            }

            //menuList.Insert(0, new MenuViewModel()
            //{
            //    MenuName = "Home",
            //    MenuUrl = "",
            //    SubMenu = new List<MenuViewModel>()
            //});
            var finalMenu = menuList;
            HttpContext.Session.Set("UserMenuList", finalMenu);
            HttpContext.Session.Set("PermissionList", permissionList);
        }

        #endregion

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IConfiguration configuration, IDetectionService detectionService)
        {
            _detectionService = detectionService;
            _logger = logger;            
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            redirectUrl = _configuration.GetSection("AppSettings").GetSection("LoginUrl").Value;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index([FromQuery] string USERNAME, [FromQuery] string PASSWD, [FromQuery] string USERID,
            [FromQuery] string USERGROUP, [FromQuery] string PROJECTSID, [FromQuery] string PINNO, [FromQuery] string USERFULLNAME,
            [FromQuery] string Acc_No, [FromQuery] string IsDChequer)
        {
            try
            {
                if (GetWebBrowserName().ToUpper() == "INTERNETEXPLORER")
                {
                    return Redirect("/Error/BrowserIssue");
                }

                if (User.Identity.IsAuthenticated)
                {
                    return View();
                }
                else
                {
                    USERNAME = HttpUtility.HtmlEncode(USERNAME);
                    PASSWD = HttpUtility.HtmlEncode(PASSWD);
                    USERID = HttpUtility.HtmlEncode(USERID);
                    USERGROUP = HttpUtility.HtmlEncode(USERGROUP);
                    PROJECTSID = HttpUtility.HtmlEncode(PROJECTSID);
                    PINNO = HttpUtility.HtmlEncode(PINNO);
                    USERFULLNAME = HttpUtility.HtmlEncode(USERFULLNAME);
                    Acc_No = HttpUtility.HtmlEncode(Acc_No);
                    IsDChequer = HttpUtility.HtmlEncode(IsDChequer);


                    if (string.IsNullOrEmpty(USERNAME))
                    {
                        return Redirect(redirectUrl);
                    }
                    else
                    {
                        var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                        {
                            AllowRefresh = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(25),
                            IsPersistent = true,
                            IssuedUtc = DateTimeOffset.UtcNow,
                            RedirectUri = redirectUrl
                        };

                        var userInfo = await _unitOfWork.UserRepo.GetUserInfoByUserName(USERNAME);

                        var groupName = await _unitOfWork.UserRepo.GetProbasiByUser(USERID);
                        string gName = string.Empty;
                        if (groupName.Count > 0) {
                             gName = groupName[0].GroupName;
                        }
                        
                        string IsStatementTrue = "";

                        if (userInfo.Count > 0)
                        {
                            IsStatementTrue = userInfo[0].IsStatementTrue;
                        }
                        else
                        {
                            return Redirect(redirectUrl + "?b=" + SecureRandomNumberGenerator.SecureRandomNo().ToString());
                        }

                        string projectName = await _unitOfWork.UserRepo.GetProjectName(PROJECTSID);

                        if (projectName == null)
                        {
                            return Redirect(redirectUrl + "?b=" + SecureRandomNumberGenerator.SecureRandomNo().ToString());
                        }

                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, USERNAME),
                    new Claim("USERFULLNAME", string.IsNullOrEmpty(USERFULLNAME)?"":USERFULLNAME),
                    new Claim("USERGROUP", string.IsNullOrEmpty(USERGROUP)?"": USERGROUP),
                    new Claim("USERID", string.IsNullOrEmpty(USERID)?"":USERID),
                    new Claim("PROJECTSID", string.IsNullOrEmpty(PROJECTSID)?"":PROJECTSID),
                    new Claim("PASSWD", string.IsNullOrEmpty(PASSWD)?"":PASSWD),
                    new Claim("IsStatementTrue", string.IsNullOrEmpty(IsStatementTrue)?"":IsStatementTrue),
                    new Claim("ProjectName", string.IsNullOrEmpty(projectName)?"" : projectName),
                    new Claim("GroupName", string.IsNullOrEmpty(gName)?"" : "Y")
                };
                        var claimsIdentity = new ClaimsIdentity(claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                        await GetUserMenuList(USERID, PROJECTSID, USERGROUP);

                        await _unitOfWork.CustomerMemoRepo.InsertXCRVLog("Login", "Post", "/Home", ""
                       , "200", "", USERID, USERFULLNAME, PROJECTSID, string.Empty);

                        if (IsDChequer != null && Acc_No != null && IsLocalUrl(System.Net.WebUtility.HtmlEncode(Acc_No)))
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    HttpContext.Session.Clear();
                    return Redirect(redirectUrl);
                }
                catch
                {
                    HttpContext.Session.Clear();
                    return Redirect(redirectUrl);
                }
            }
        }

        [Route("Logout")]
       // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();
            return Redirect(redirectUrl);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });


        }


    }
}
