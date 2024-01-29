using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;

namespace XCRV.Web.Controllers
{
    public class AccessCodeController : BaseController
    {

        private readonly ILogger<AccessCodeController> _logger;

        private readonly IUnitOfWork _unitOfWork;

        public AccessCodeController(ILogger<AccessCodeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Entry()
        {           

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetAccessCodeList()
        {
            IList<FinacleUser> data = new List<FinacleUser>();

            string message = "Sorry!!! No Data Found!!!";

            data = await _unitOfWork.AccessKeyRepo.GetAccessInfoList();           

            return Json(new { data = data, status = "success", message = message, result = CommonAjaxResponse("Success", "Success", "200") });

        }


        public async Task<JsonResult> GetFinacleUserId()
        {
            return Json(await _unitOfWork.AccessKeyRepo.GetFinacleUserList());
        }

        public async Task<JsonResult> GetXcrvFinacleUserId()
        {
            return Json( await _unitOfWork.UserRepo.GetXcrvUserName());
        }

        public async Task<ActionResult> SaveAccessCode(FinacleUser requst)
        {
            string message = "";
            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();           
            bool saved = false;

            if (requst.acct_access_code == "")
            {
                message = "Access Code can not be empty.";
                
            }
            else if (requst.user_id.Trim() == "" || requst.user_id.Trim().ToString() == "0")
            {
                message = "User ID can not be empty.";
                
            }
            else if (requst.xcrv_user_id.Trim() == "" || requst.xcrv_user_id.Trim() == "0")
            {
                message = "XCRV User ID can not be empty.";
            }
            else
            {
                if (await _unitOfWork.AccessKeyRepo.GetIsExistAccessCode(requst) == null)
                {
                    if (string.IsNullOrEmpty(requst.access_id))
                    {
                        await _unitOfWork.AccessKeyRepo.SaveAccessInfo(requst, userName);
                        saved = true;
                        message = "Successfully Inserted!!!";
                    }
                    else
                    {
                        await _unitOfWork.AccessKeyRepo.UpdateAccessCode(requst, userName);
                        saved = true;
                        message = "Successfully Updated!!!";
                    }
                }
                else
                {
                    message = "Same access code already exists for user";
                }
            }

            
            return Json(new { status = saved, message = message });
        }


        public async Task<ActionResult> DeleteAccessCode(FinacleUser requst)
        {
            string message = "";
            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            bool saved = false;
           
            await _unitOfWork.AccessKeyRepo.DeleteAccessCode(requst, userName);
            saved = true;
            message = "Successfully deleted!!!";

            return Json(new { status = saved, message = message });
        }
    }
}
