using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Application.Interfaces;

namespace XCRV.Web.Views.Shared.Components.TopMenuWidget
{
    public class TopMenuWidgetViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public TopMenuWidgetViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
           // var lastUpdatedDate = DateTime.Today;
            var lastUpdatedDate = await _unitOfWork.OracleBaseRepo.GetLastUpdatedDate();

            string pstrCrd = HttpContext.Session.Get<string>("crdInd");

            string label = string.Empty;
            if (string.IsNullOrEmpty(pstrCrd))
                label = "Finacle Data available as of " + lastUpdatedDate.ToString("dd MMM yy");
            else
            {
                label = "Cardpro Data available as of yesterday.";
                HttpContext.Session.Set("crdInd", string.Empty);
            }
            return View("_TopNavigation", label);
        }
    }
}
