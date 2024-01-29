using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using XCRV.Application.Interfaces;

namespace XCRV.Web.Middleware
{
    public class SerilogMiddleware
    {
        const string MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} {QueryString} responded {StatusCode} in {Elapsed:0.0000} ms {byUser}";

        static readonly ILogger Log = Serilog.Log.ForContext<SerilogMiddleware>();
        private readonly IUnitOfWork _unitOfWork;

        readonly RequestDelegate _next;

        public SerilogMiddleware(RequestDelegate next, IUnitOfWork unitOfWork)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));
            _next = next;
            _unitOfWork = unitOfWork;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string queryString = string.Empty;
            string userName = string.Empty;
            string userID = string.Empty;
            string projectId = string.Empty;

            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));



            var sw = Stopwatch.StartNew();
            try
            {
                await _next(httpContext);
                sw.Stop();

                var statusCode = httpContext.Response?.StatusCode;
                var level = statusCode > 499 ? LogEventLevel.Error : LogEventLevel.Information;

               
                if (httpContext.User != null && httpContext.User.Identity.IsAuthenticated)
                {
                    userID = httpContext.User.Claims.FirstOrDefault(p => p.Type.Equals("USERID")).Value.ToString();
                    userName = "By " +httpContext.User.Identity.Name + "("+ userID + ")";
                    projectId = httpContext.User.Claims.FirstOrDefault(p => p.Type.Equals("PROJECTSID")).Value.ToString();
                    
                }
                

                var log = level == LogEventLevel.Error ? LogForErrorContext(httpContext) : Log;

                

                if(!httpContext.Request.QueryString.ToString().Contains("PASSWD"))
                {
                    
                    queryString = httpContext.Request.QueryString.ToString();
                }
                {
                    queryString = RemoveQueryStringByKey("https://www.xcrv.bracbank.com" + httpContext.Request.QueryString.ToString(), "PASSWD");
                    queryString = queryString.Replace("https://www.xcrv.bracbank.com", "");
                }
                


                log.Write(level, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, queryString, statusCode, sw.Elapsed.TotalMilliseconds, userName);

                await _unitOfWork.CustomerMemoRepo.InsertXCRVLog(level.ToString(), httpContext.Request.Method, httpContext.Request.Path, queryString
                    , statusCode.ToString(), sw.Elapsed.TotalMilliseconds.ToString(),userID, userName, projectId, string.Empty);
            }
            // Never caught, because `LogException()` returns false.
            catch (Exception ex) when (LogException(httpContext, sw, ex)) {

                await _unitOfWork.CustomerMemoRepo.InsertXCRVLog(LogEventLevel.Error.ToString(), httpContext.Request.Method, httpContext.Request.Path, queryString
                        , "500".ToString(), sw.Elapsed.TotalMilliseconds.ToString(), userID, userName, projectId, string.Empty);
            }
        }

        static bool LogException(HttpContext httpContext, Stopwatch sw, Exception ex)
        {
            sw.Stop();

            
            LogForErrorContext(httpContext)
                .Error(ex, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, 500, sw.Elapsed.TotalMilliseconds);


            return false;
        }

        static ILogger LogForErrorContext(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var result = Log
                .ForContext("RequestHeaders", request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
                .ForContext("RequestHost", request.Host)
                .ForContext("RequestProtocol", request.Protocol);

            if (request.HasFormContentType)
                result = result.ForContext("RequestForm", request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()));

            return result;
        }

         static string RemoveQueryStringByKey(string url, string key)
        {
            var uri = new Uri(url);

            // this gets all the query string key value pairs as a collection
            var newQueryString = System.Web.HttpUtility.ParseQueryString(uri.Query);

            // this removes the key if exists
            newQueryString.Remove(key);

            // this gets the page path from root without QueryString
            string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

            return newQueryString.Count > 0
                ? String.Format("{0}?{1}", pagePathWithoutQueryString, newQueryString)
                : pagePathWithoutQueryString;
        }
    }
}
