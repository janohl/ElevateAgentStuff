using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class BaseController : Controller
    {
        protected log4net.ILog Logger;   
        public BaseController()
        {
            Logger = log4net.LogManager.GetLogger(this.GetType().Name);   
        }

    }
}