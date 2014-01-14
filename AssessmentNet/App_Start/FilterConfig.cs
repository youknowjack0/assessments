using System.Diagnostics;
using System.Web;
using System.Web.Mvc;

namespace AssessmentNet
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //var handler = new MyErrorHandler();
            filters.Add(new HandleErrorAttribute());
        }
    }

    public class MyErrorHandler : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            Trace.TraceError(filterContext.Exception.Message);
            Trace.TraceError(filterContext.Exception.Source);
            Trace.TraceError(filterContext.Exception.StackTrace);

            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml"
            };
            filterContext.ExceptionHandled = true;
        }
    }
}
