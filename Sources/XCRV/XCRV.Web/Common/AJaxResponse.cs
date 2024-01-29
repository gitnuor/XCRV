using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XCRV.Web.Common
{
    public class AJaxResponse : IDisposable
    {
        public string _DisplayMessageType;
        public string _DisplayMessage;
        public string _AjaxCode;

        public void Dispose()
        {

        }
    }

}
