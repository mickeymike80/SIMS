using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIMS_CW.Controllers
{
    public class ManagerController : Controller
    {
        // GET: Manager
        public ActionResult LineChart()
        {
            return View();
        }
    }
}