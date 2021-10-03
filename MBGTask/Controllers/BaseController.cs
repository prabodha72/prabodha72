using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MBGTask.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        public enum ErrorType
        {
            warning,
            success,
            error
        }
        public void showErrorMessage(string Error, ErrorType ErrorType)
        {
            if(ErrorType == ErrorType.warning && Error != "")
            {
                ViewBag.errormsg = Error;
                ViewBag.errortype = "warning";
            }
            else if(ErrorType == ErrorType.success && Error != "")
            {
                ViewBag.errormsg = Error;
                ViewBag.errortype = "success";
            }
            else if (ErrorType == ErrorType.error && Error != "")
            {
                ViewBag.errormsg = Error;
                ViewBag.errortype = "error";
            }
        }
    }
}