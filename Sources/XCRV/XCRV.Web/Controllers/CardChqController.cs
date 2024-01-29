using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XCRV.Application.Interfaces;
using XCRV.Domain.Entities;
using XCRV.Web.Helpers;
using XCRV.Web.Models;

namespace XCRV.Web.Controllers
{
    public class CardChqController : BaseController
    {
        private readonly ILogger<CardChqController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public CardChqController(ILogger<CardChqController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult DisplayImage(string ImageID)
        {
            var ImageIds = HttpUtility.HtmlEncode(ImageID).Split(',');
            string imageID = ImageIds[0];
            string imageType = ImageIds[1];  // Photo or Signature
            string cardCheckImagePath = _configuration.GetSection("AppSettings").GetSection(imageType).Value; //System.Configuration.ConfigurationManager.AppSettings[imageType].ToString();
            string filePath = cardCheckImagePath + imageID.Trim() + ".jpg";
            if (System.Diagnostics.Debugger.IsAttached)
            {
                filePath = "D:\footer.png";
            }


            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            Byte[] bytes = br.ReadBytes((Int32)fs.Length);
            br.Close();
            fs.Close();


            return File(bytes, "image/png");
        }


        [Filters.AuthorizeActionFilter]
        public IActionResult Active()
        {
            return View();
        }

        public async Task<ActionResult> ShowCustomerInfo(string refNo, string submited)
        {
            try
            {
                refNo = HttpUtility.HtmlEncode(refNo);
                
                submited = HttpUtility.HtmlEncode(submited);

                CardCustomerInformation chqCustomer = null;
                IList<ChqBookInfo> chqBookInfos = null;
                CardChqViewModel viewModel = new CardChqViewModel();

                if (string.IsNullOrEmpty(submited))
                {
                    if (chqCustomer == null)
                    {
                        chqCustomer = new CardCustomerInformation();
                        chqBookInfos = new List<ChqBookInfo>();
                    }
                    
                    viewModel.CardCustomerInformation = chqCustomer;
                    viewModel.ChqBookInfos = chqBookInfos;
                    return PartialView("_CardProCustomerInfo", viewModel);
                }

                string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

                


                if (string.IsNullOrEmpty(refNo))
                {
                    ViewBag.ErrorMessage = "Referance Number can not be empty.";
                }
                
                else if (!this.IsNumberString(refNo))
                {
                    ViewBag.ErrorMessage = "Referance Number Can't contain Special/Normal characters.";
                }
                else
                {
                    try
                    {
                        chqCustomer = await _unitOfWork.QmsCardProRepo.GetCustomerInformationByRefNo(refNo);

                        if(chqCustomer != null)
                        {
                            if (string.IsNullOrEmpty(chqCustomer.CB_PLASTIC_CD.Trim()))
                            {
                                chqCustomer.STATUS = "Active/Open";
                            }                                
                            else
                            {
                                chqCustomer.STATUS = chqCustomer.CB_PLASTIC_CD;
                            }
                                

                            string strDOB = chqCustomer.CB_DOB;
                            string strYear = strDOB.Substring(0, 4);
                            string strMonth = strDOB.Substring(4, 2);
                            string strDay = strDOB.Substring(6, 2);
                            chqCustomer.CB_DOB = new DateTime(Convert.ToInt32(strYear), Convert.ToInt32(strMonth), Convert.ToInt32(strDay)).ToLongDateString();

                            string ImageID = refNo.Trim() + "," + "CustomerImage";
                            chqCustomer.PhotoUrl = "/CardChq/DisplayImage?ImageID=" + ImageID;

                            string SignatureID = refNo.Trim() + "," + "CustomerSignature";
                            chqCustomer.SignatureUrl = "/CardChq/DisplayImage?ImageID=" + SignatureID;
                            
                            string supervisorID = "0";
                            string subOrdinateID = "0";
                            string cardno = "";
                            string chqno = "";
                            string fromdate = "";
                            string todate = "";
                            string remarks = "";
                            string intQueryType = "1";

                            chqBookInfos = await _unitOfWork.CardChqRepo.GetUpdatedChqBookData(refNo, userName, supervisorID, subOrdinateID, cardno, chqno, fromdate, todate, remarks, intQueryType);
                            if(submited.Equals("submited"))
                            {
                                if (chqBookInfos.Count > 0)
                                {
                                    ViewBag.ChqBookMessage = "Cheque book information found!!!";
                                }
                                else
                                {
                                    ViewBag.ChqBookErrorMessage = "No cheque book information found!!!";
                                }
                            }                            
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Data Not Found!!!!";
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorMessage = ex.Message;
                    }
                }

                if (chqCustomer == null)
                {
                    chqCustomer = new CardCustomerInformation();
                    chqBookInfos = new List<ChqBookInfo>();
                }
                                
                viewModel.CardCustomerInformation = chqCustomer;
                viewModel.ChqBookInfos = chqBookInfos;

                return PartialView("_CardProCustomerInfo", viewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> ActivateChqBook(CardChqBookActiveRequst requst)
        {
            string message = "";
            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            string supervisorID = "0";
            string subOrdinateID = "0";
            string cardno = "";
            string chqno = requst.chqNo;
            string fromdate = "";
            string todate = "";
            string remarks = requst.remarks;
            string intQueryType = "2";
            string refNo = "";

            bool saved = false;
            
            await _unitOfWork.CardChqRepo.GetUpdatedChqBookData(refNo, userName, supervisorID, subOrdinateID, cardno, chqno, fromdate, todate, remarks, intQueryType);
            saved = true;
            message = "Cheque Book Activated Successfully!!!";

            return Json(new { status = saved, message = message });
        }

        [Filters.AuthorizeActionFilter]
        public IActionResult ActiveVerify()
        {
            return View();
        }


        public async Task<ActionResult> ShowChqBookInfoForVerification(CardChqBookVerifyReqeust requst)
        {
            try
            {
                requst.submited = HttpUtility.HtmlEncode(requst.submited);

                IList<CardChqActiveVerify> viewModel = new List<CardChqActiveVerify>();

                if (string.IsNullOrEmpty(requst.submited))
                {
                   
                    return PartialView("_PendingActiveChqBook", viewModel);
                }

                string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                                                
                
                try
                {
                    viewModel = await _unitOfWork.CardChqRepo.GetChqBookDataForVerify(requst.refNo, "0", userName, requst.userID, requst.cardNo, requst.chqNo, requst.frmDate, requst.todate, requst.remarks);


                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                }
                
                return PartialView("_PendingActiveChqBook", viewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<ActionResult> VerifyActiveChqBook(CardChqBookVerifyReqeust requst)
        {
            string message = "";
            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            string supervisorID = "0";
            string subOrdinateID = "0";
            string cardno = "";
            string chqno = requst.chqNo;
            string fromdate = "";
            string todate = "";
            string remarks = requst.remarks;            
            string refNo = "";

            bool saved = false;

            if(await _unitOfWork.CardChqRepo.VerifyChqBookData(refNo, userName, supervisorID, subOrdinateID, cardno, chqno, fromdate, todate, remarks))
            {
                message = "Card No :" + requst.cardNo + "(CHQ No: " + requst.chqNo + ") Already Approved.";
            }
            else
            {
                remarks = requst.remarks;
                await _unitOfWork.CardChqRepo.ApproveChqBookData(refNo, userName, supervisorID, subOrdinateID, cardno, chqno, fromdate, todate, remarks);
                saved = true;
                message = "Cheque Book Activated Successfully!!!";

            }

            return Json(new { status = saved, message = message });
        }


        public async Task<ActionResult> VerifyActiveChqBookList(IList<CardChqBookVerifyReqeust> requst)
        {
            bool saved = false;
            string message = "";
            string checkValidationMessage = string.Empty;
            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            string supervisorID = "0";
            string subOrdinateID = "0";
            string cardno = "";
            string fromdate = "";
            string todate = "";
            string refNo = "";
            int i = 0;
            foreach (var data in requst)
            {
                string chqno = data.chqNo;
                string remarks = data.remarks;
                if (await _unitOfWork.CardChqRepo.VerifyChqBookData(refNo, userName, supervisorID, subOrdinateID, cardno, chqno, fromdate, todate, remarks))
                {
                    message = "Card No :" + data.cardNo + "(CHQ No: " + data.chqNo + ") Already Approved.";
                    checkValidationMessage = checkValidationMessage == string.Empty ?
                                checkValidationMessage + message : checkValidationMessage + " and " + message;
                }
                else
                {
                    remarks = data.remarks;
                    await _unitOfWork.CardChqRepo.ApproveChqBookData(refNo, userName, supervisorID, subOrdinateID, cardno, chqno, fromdate, todate, remarks);
                    saved = true;
                    i = i + 1;
                    message = "Cheque Book Activated Successfully!!!";
                }
            }

            checkValidationMessage = checkValidationMessage != string.Empty ? " But " + checkValidationMessage + " Already Approved." : checkValidationMessage;
            message = i + " data Approved." + checkValidationMessage;
            return Json(new { status = saved, message = message });
        }


        [Filters.AuthorizeActionFilter]
        public IActionResult Deactive()
        {
            return View();
        }

        public async Task<ActionResult> ShowCustomerInfoForDeActive(string refNo, string submited)
        {
            try
            {
                refNo = HttpUtility.HtmlEncode(refNo);

                submited = HttpUtility.HtmlEncode(submited);

                CardCustomerInformation chqCustomer = null;
                IList<ChqBookInfo> chqBookInfos = null;
                CardChqViewModel viewModel = new CardChqViewModel();

                if (string.IsNullOrEmpty(submited))
                {
                    if (chqCustomer == null)
                    {
                        chqCustomer = new CardCustomerInformation();
                        chqBookInfos = new List<ChqBookInfo>();
                    }

                    viewModel.CardCustomerInformation = chqCustomer;
                    viewModel.ChqBookInfos = chqBookInfos;
                    return PartialView("_CardProCustomerInfoForDeactive", viewModel);
                }

                string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();




                if (string.IsNullOrEmpty(refNo))
                {
                    ViewBag.ErrorMessage = "Referance Number can not be empty.";
                }

                else if (!this.IsNumberString(refNo))
                {
                    ViewBag.ErrorMessage = "Referance Number Can't contain Special/Normal characters.";
                }
                else
                {
                    try
                    {
                        chqCustomer = await _unitOfWork.QmsCardProRepo.GetCustomerInformationByRefNo(refNo);

                        if (chqCustomer != null)
                        {
                            if (string.IsNullOrEmpty(chqCustomer.CB_PLASTIC_CD.Trim()))
                            {
                                chqCustomer.STATUS = "Active/Open";
                            }
                            else
                            {
                                chqCustomer.STATUS = chqCustomer.CB_PLASTIC_CD;
                            }


                            string strDOB = chqCustomer.CB_DOB;
                            string strYear = strDOB.Substring(0, 4);
                            string strMonth = strDOB.Substring(4, 2);
                            string strDay = strDOB.Substring(6, 2);
                            chqCustomer.CB_DOB = new DateTime(Convert.ToInt32(strYear), Convert.ToInt32(strMonth), Convert.ToInt32(strDay)).ToLongDateString();

                            string ImageID = refNo.Trim() + "," + "CustomerImage";
                            chqCustomer.PhotoUrl = "/CardChq/DisplayImage?ImageID=" + ImageID;

                            string SignatureID = refNo.Trim() + "," + "CustomerSignature";
                            chqCustomer.SignatureUrl = "/CardChq/DisplayImage?ImageID=" + SignatureID;

                            string chqBookSL = "1";
                            string remarks = string.Empty;
                            string chqStatus = "1";
                            string queryType = "2";

                            var issuedChqBooks = await _unitOfWork.CardChqRepo.GetIssuedChqBookData(refNo,chqBookSL,remarks,chqStatus, userName, queryType);
                            issuedChqBooks.Insert(0, new CardChqEntity() { numChqBookSL = "-99", strChqBookName = "Select" });

                            ViewBag.IssuedChqBooks = issuedChqBooks
                                     .Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                                     {
                                         Value = i.numChqBookSL.ToString(),
                                         Text = i.strChqBookName
                                     }).ToList();

                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Data Not Found!!!!";
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorMessage = ex.Message;
                    }
                }

                if (chqCustomer == null)
                {
                    chqCustomer = new CardCustomerInformation();
                    chqBookInfos = new List<ChqBookInfo>();
                }

                viewModel.CardCustomerInformation = chqCustomer;
                viewModel.ChqBookInfos = chqBookInfos;

                return PartialView("_CardProCustomerInfoForDeactive", viewModel);
            }
            catch (Exception ex)
            {
                throw ;
            }
        }

        public async Task<ActionResult> ShowCustomerChqBookDetails(string chqBookSL)
        {
            try
            {
                chqBookSL = HttpUtility.HtmlEncode(chqBookSL);
                CustomerChqBookViewModel viewModel = null;
                string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                if (string.IsNullOrEmpty(chqBookSL))
                {
                    TempData["ErrorMessage"] = "Chq Book Number can not be empty.";
                }               
                else
                {
                    try
                    {
                        viewModel = new CustomerChqBookViewModel();
                        viewModel.ChqBookInfo = (await _unitOfWork.CardChqRepo.GetIssuedChqBookData(refNo: string.Empty, chqBookSL:chqBookSL,remarks:string.Empty
                            , chqStatus: "1", userID: userName, queryType:"3")).FirstOrDefault();

                        if (viewModel.ChqBookInfo != null)
                        {
                            viewModel.ChqBookDetails = (await _unitOfWork.CardChqRepo.GetIssuedChqBookData(refNo: string.Empty, chqBookSL: chqBookSL, remarks: string.Empty
                            , chqStatus: "1", userID: userName, queryType: "4")).ToList();

                        }
                        else
                        {
                            viewModel.ChqBookInfo = new CardChqEntity();
                            viewModel.ChqBookDetails = new List<CardChqEntity>();

                            TempData["ErrorMessage"] = "Data Not Found!!!!";                            
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = ex.Message;
                    }
                }

                if (viewModel == null)
                {
                    viewModel = new CustomerChqBookViewModel();
                    viewModel.ChqBookInfo = new CardChqEntity();
                    viewModel.ChqBookDetails = new List<CardChqEntity>();                    
                }

                return Json(new { data = viewModel, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public JsonResult GetChqActionStatuses(bool forFullBook)
        {
            return Json(new ChqActionStatus().GetChqActionStatuses(forFullBook));
        }


        public async Task<ActionResult> DeactiveChequeBookLeaf(CardDeactiveEntity requst)
        {

            bool saved = false;
            string message = string.Empty;
            if (!string.IsNullOrEmpty(requst.paymentStatus) && Convert.ToInt16(requst.paymentStatus.Trim()) > 0)
            {
                message = "Cheque alredy placed for processing.";               
            }

            else if (!string.IsNullOrEmpty(requst.chqCancelStatus)  && (Convert.ToInt32(requst.chqCancelStatus.Trim()) == 1 || Convert.ToInt32(requst.chqCancelStatus.Trim()) == 3 || Convert.ToInt32(requst.chqCancelStatus.Trim()) == 4))
            {
                

                if (Convert.ToInt32(requst.chqCancelStatus.Trim()) == 1)
                {
                    message = "Cheque canceled you are not authorized to proceed.";
                }
                else if (Convert.ToInt32(requst.chqCancelStatus.Trim()) == 3)
                {
                    message = "Cheque lost you are not authorized to proceed.";
                }
                else if (Convert.ToInt32(requst.chqCancelStatus.Trim()) == 4)
                {
                    message = "Cheque bounced you are not authorized to proceed.";
                }
            }

            else if (requst.intChqStatus == "-99")
            {
                message = "Please select any action.";
                
            }
            else if (requst.strCancelRemarks.Trim() == string.Empty)
            {
                message = "Remarks can not be empty.";
                
            }
            else
            {
                string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString(); ;
                if (requst.forFullBook)
                {
                    requst.intUserID = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                    requst.intQueryType = "2";
                    requst.strCancelRemarks = "Whole book cancel, " + requst.strCancelRemarks;

                    if(requst.intChqStatus.Trim() == "1")
                    {
                        await _unitOfWork.CardChqRepo.ChequeBookCancelEntry(requst);
                        saved = true;
                        message = "Saved Successfully!!!";
                    }
                    else
                    {
                        var issuedChqBook = (await _unitOfWork.CardChqRepo.GetIssuedChqBookData(refNo: string.Empty, chqBookSL: requst.numChqBookSL, remarks: string.Empty
                            , chqStatus: "1", userID: userName, queryType: "4")).ToList();

                        List<CardDeactiveEntity> list = new List<CardDeactiveEntity>();

                        foreach (var data in issuedChqBook)
                        {
                            CardDeactiveEntity entity = new CardDeactiveEntity
                            {
                                numChqBookSL = data.numChqNo.Remove(0, 4).Trim(),
                                intChqStatus = requst.intChqStatus,
                                intUserID = userName,
                                strCancelRemarks = requst.txtChqStatus.Trim() + "," + requst.strCancelRemarks.Trim(),
                                intQueryType = "1"
                            };
                            list.Add(entity);
                        }

                        saved = await _unitOfWork.CardChqRepo.ChequeBookCancelEntry(list);
                        message = "Saved Successfully!!!"; 

                    }
                }
                else
                {
                    requst.numChqBookSL = requst.numChqBookSL.Trim().Remove(0, 4);
                    requst.intUserID = userName;
                    requst.intQueryType = "1";

                    await _unitOfWork.CardChqRepo.ChequeBookCancelEntry(requst);
                    saved = true;
                    message = "Saved Successfully!!!";
                }
            }
            
            return Json(new { status = saved, message = message });
        }


        public async Task<ActionResult> DeactiveMultiChequeBookLeaf(IList<CardDeactiveEntity> requst)
        {

            bool saved = false;
            string message = string.Empty;
            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
            foreach (var data in requst)
            {
                data.numChqBookSL = data.numChqBookSL.Remove(0, 4).Trim();
                data.intUserID = userName;
                data.intQueryType = "1";

            }

            saved = await _unitOfWork.CardChqRepo.ChequeBookCancelEntry(requst);
            message = "Saved Successfully!!!";
            
            return Json(new { status = saved, message = message });
        }

        [Filters.AuthorizeActionFilter]
        public IActionResult DeactiveVerify()
        {
            return View();
        }
        
        public async Task<JsonResult> GetUserId()
        {
            return Json(await _unitOfWork.CardChqRepo.GetUserList());
        }

        public async Task<ActionResult> ShowPendingCancelChequeBookData(CardChqBookVerifyReqeust requst)
        {
            try
            {
                string message = "";
                requst.userID= string.IsNullOrEmpty( requst.userID)  ? "-99" : requst.userID;

                requst.cardNo = string.IsNullOrEmpty(requst.cardNo) ? "-99" : requst.cardNo;

                IList<CardChequeDeactiveVerifyEntity> viewModel = new List<CardChequeDeactiveVerifyEntity>();
                string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                
                viewModel = await _unitOfWork.CardChqRepo.GetPendingCancelChequeBookData(requst.cardNo, requst.frmDate,requst.todate);

               // viewModel = await _unitOfWork.CardChqRepo.GetPendingCancelChequeBookData(requst.userID, userName)
 
                 string date = !string.IsNullOrEmpty( requst.frmDate) ? DateTime.Parse(requst.frmDate).ToString("dd-MMM-yyyy") : requst.frmDate;
                string ToDate = !string.IsNullOrEmpty(requst.todate) ? DateTime.Parse(requst.todate).ToString("dd-MMM-yyyy") : requst.todate;

                //var liqList = (from bk in viewModel
                //               where
                //                   ((DateTime.Parse(bk.dtCancelDate) >= DateTime.Parse(date) && DateTime.Parse(bk.dtCancelDate) <= DateTime.Parse(ToDate)))
                //                   && (!string.IsNullOrEmpty(requst.cardNo) || bk.strCardNo == requst.cardNo)
                //               select new
                //               {
                //                   bk.nric_number,
                //                   bk.strCusName,
                //                   bk.strCardNo,
                //                   bk.strChqBookName,
                //                   bk.numChqName,
                //                   bk.numChqBookSL,
                //                   bk.numChqNo,
                //                   bk.ChequeStatus,
                //                   bk.ChequeCancelStatus,
                //                   bk.strCancelRemarks,
                //                   bk.dtCancelDate,
                //                   bk.intCancelBy,
                //                   bk.UserName
                //               }).OrderByDescending(c => c.dtCancelDate);

                return Json(new { data = viewModel, status = "success", result = CommonAjaxResponse("Success", "Success", "200") });

                
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                throw;
            }
        }


        public async Task<ActionResult> CancelVerifyCheck(IList<CardChequeDeactiveVerifyEntity> requst)
        {

            bool saved = false;
            string message = string.Empty;
            string userName = User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();

            string mgs = string.Empty;
            string checkValidationMessage = string.Empty;

            foreach (var data in requst)
            {
                data.intUserID = userName;

                var list = await _unitOfWork.CardChqRepo.CancelVerifyCheck(data.numChqBookSL, data.numChqNo);
                if (list != null && list.Count > 0)
                {
                    mgs = "Card No :" + list[0].strCardNo + "( CHQ No:" + list[0].numChqName + ")";
                }

                if (mgs != string.Empty)
                {
                    checkValidationMessage = checkValidationMessage == string.Empty ?
                        checkValidationMessage + mgs : checkValidationMessage + " and " + mgs;
                }
                else
                {
                    bool done = await _unitOfWork.CardChqRepo.CancelVerifyEntry(data);
                }
            }

            saved = true;

            checkValidationMessage = checkValidationMessage != string.Empty ? " But " + checkValidationMessage + " Already verifyed." : checkValidationMessage;
            message = "Data Updated Successfully." + checkValidationMessage;

            return Json(new { status = saved, message = message });
        }


        [Filters.AuthorizeActionFilter]
        public IActionResult Report()
        {
            return View();
        }


        public async Task<IActionResult> ShowChqActiveDeactiveReport(CardChqReportRequest request)
        {
            string message = "";

            IList<CardChqReport> list = new List<CardChqReport>();

            

            if(string.IsNullOrEmpty(request.submited))
            {
                return PartialView("_ActiveDeactiveReport", list);
            }

            CultureInfo enUS = new CultureInfo("en-US");
            DateTime fDate = new DateTime();
            DateTime tDate = new DateTime();

            request.cardNo = string.IsNullOrEmpty(request.cardNo) ? "" : HttpUtility.HtmlEncode(request.cardNo);
            request.chqNo = string.IsNullOrEmpty(request.chqNo) ? "" : HttpUtility.HtmlEncode(request.chqNo);
            request.userId = string.IsNullOrEmpty(request.userId) ? "-1" : HttpUtility.HtmlEncode(request.userId);

            if (string.IsNullOrEmpty(request.frmDate) && !string.IsNullOrEmpty(request.submited))
            {
                message = "From date can not be empty.";
            }
            else if (!string.IsNullOrEmpty(request.frmDate) && !string.IsNullOrEmpty(request.submited))
            {
                if (!DateTime.TryParseExact(request.frmDate.Trim(), "dd-MMM-yyyy", enUS, DateTimeStyles.None, out fDate))
                {
                    message = "From date is not in correct format. Date format dd-MMM-yyyy (01-Dec-2009)";
                }
            }

            if (string.IsNullOrEmpty(request.toDate) && !string.IsNullOrEmpty(request.submited))
            {
                message = "To date can not be empty.";
            }
            else if (!string.IsNullOrEmpty(request.toDate) && !string.IsNullOrEmpty(request.submited))
            {
                if (!DateTime.TryParseExact(request.toDate.Trim(), "dd-MMM-yyyy", enUS, DateTimeStyles.None, out tDate))
                {
                    message = "To date is not in correct format. Date format dd-MMM-yyyy (01-Dec-2009)";
                }
            }

            list = await _unitOfWork.CardChqRepo.CardChqActiveDeactiveReport(request.cardNo,request.chqNo, request.frmDate, request.toDate, request.userId, request.reportType);

            ViewBag.ErrorMessage = message;
            return PartialView("_ActiveDeactiveReport", list);

            
        }

        public async Task<IActionResult> ShowChqActiveDeactiveExcelReport(CardChqReportRequest request)
        {
            string message = "";

            IList<CardChqReport> list = new List<CardChqReport>();

            CultureInfo enUS = new CultureInfo("en-US");
            DateTime fDate = new DateTime();
            DateTime tDate = new DateTime();

            request.cardNo = string.IsNullOrEmpty(request.cardNo) ? "" : HttpUtility.HtmlEncode(request.cardNo);
            request.chqNo = string.IsNullOrEmpty(request.chqNo) ? "" : HttpUtility.HtmlEncode(request.chqNo);
            request.userId = string.IsNullOrEmpty(request.userId) ? "-1" : HttpUtility.HtmlEncode(request.userId);

            if (string.IsNullOrEmpty(request.frmDate) && !string.IsNullOrEmpty(request.submited))
            {
                message = "From date can not be empty.";
            }
            else if (!string.IsNullOrEmpty(request.frmDate) && !string.IsNullOrEmpty(request.submited))
            {
                if (!DateTime.TryParseExact(request.frmDate.Trim(), "dd-MMM-yyyy", enUS, DateTimeStyles.None, out fDate))
                {
                    message = "From date is not in correct format. Date format dd-MMM-yyyy (01-Dec-2009)";
                }
            }

            if (string.IsNullOrEmpty(request.toDate) && !string.IsNullOrEmpty(request.submited))
            {
                message = "To date can not be empty.";
            }
            else if (!string.IsNullOrEmpty(request.toDate) && !string.IsNullOrEmpty(request.submited))
            {
                if (!DateTime.TryParseExact(request.toDate.Trim(), "dd-MMM-yyyy", enUS, DateTimeStyles.None, out tDate))
                {
                    message = "To date is not in correct format. Date format dd-MMM-yyyy (01-Dec-2009)";
                }
            }

            list = await _unitOfWork.CardChqRepo.CardChqActiveDeactiveReport(request.cardNo, request.chqNo, request.frmDate, request.toDate, request.userId, request.reportType);

            TempData["ErrorMessage"] = message;

            if (list.Count == 0)
            {
                TempData["ErrorMessage"] = "Sorry!!! No Data found.!!!";                
            }
            else
            {
                string sb = ReportHelper<CardChqReport>.ConvertExcel(list);
                string fileName = "ChqStopReport_" + DateTime.Now.ToString("MMddyyHHmmss").Trim();
                return File(Encoding.ASCII.GetBytes(sb.ToString()), "application/vnd.ms-excel", fileName + ".xls");
            }

            return RedirectToAction("Report");
        }
    }
}
