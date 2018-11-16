using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webtest.Models;

namespace webtest.Controllers
{
    public class HomeController : Controller
    {
        //Connectie met database
        DatabaseEntities1 db = new DatabaseEntities1();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WishList()
        {
            ViewBag.Message = "THIS IS THE WISHLIST PAGE";
            return View();

        }

        public ActionResult ShoppingCart(double delete = 0)
        {

            List<Book> bookList = new List<Book>();

            if (Session["shoppingCart"] != null)
            {
                List<string> isbns = Session["shoppingCart"].ToString().Split(',').ToList();

                var total = 0;
                foreach (var x in isbns)
                {
                    double add = Convert.ToDouble(x);
                    bookList.Add(db.Books.Where(m => m.ISBN == add).FirstOrDefault());
                    
                    total = total + Decimal.ToInt32(db.Books.Where(m => m.ISBN == add).Sum(m => m.Price));

                }
                ViewBag.totalPrice = total;


                if (delete != 0)
                {
                    bookList.Remove(db.Books.Where(m => m.ISBN == delete).FirstOrDefault());
                    
                }
            }

            return View(bookList);
        }

        public ActionResult OrderStatus()
        {
            ViewBag.Message = "THIS IS THE ORDER STATUS PAGE";

            if (Session["User_id"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {

                return View();
            }
        }
    }
}