using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WFPtest.Models;

namespace WFPtest.Controllers
{
    public class TreeviewController : Controller
    {
        //
        // GET: /Treeview/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Simple()
        {

            List<SiteMenu> all = new List<SiteMenu>();
            using (WFPEntities1 dc = new WFPEntities1())
            {
                all = dc.SiteMenus.OrderBy(a => a.ParentMenuID).ToList();
            }
            return View(all);
        }

    }
}