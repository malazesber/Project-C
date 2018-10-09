using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webtest.Models;
using PagedList;
using PagedList.Mvc;

namespace webtest.Controllers
{
    public class ProductController : Controller
    {
        //COnnectie met database
        DatabaseEntities1 db = new DatabaseEntities1();
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Results(string search, int? page)
        {   // !!! --- DE DATABASE MAAKT NU ALLEEN NOG GEBRUIK VAN DE USER TABLE OM TEST REDENEN. WANNEER DE JUISTE DATABASE ER IS MOET ER EEN CONNECTIE WORDEN GEMAAKT MET DE PRODUCT TABLE --- !!!

            // Queries voor producten op te halen.
            // Deze queries zijn momenteel nog een test en zullen verwijderd worden wanneer de nieuwe database er is.
            // Wanneer de nieuwe database er is hoeft hier alleen nog maar een connectie gemaakt te worden en moeten er nieuwe queries worden gemaakt.
            // De producten zullen op de jusite manier worden laten zien en de PagedList werkt ook gewoon nog.
            // Het is belangrijk dat de View Een .ToList().TopPagedList() returned zodat er door de producten geloopt kan worden en zodat de PagedList goed werkt.
            if (search == "")
            {
                return View(db.Books.Where(m => m.Name.Contains("*#*#*@")).ToList().ToPagedList(page ?? 1, 3));
            }
            return View(db.Books.Where(m => m.Name.Contains(search)).ToList().ToPagedList(page ?? 1, 3));
        }
    }
}