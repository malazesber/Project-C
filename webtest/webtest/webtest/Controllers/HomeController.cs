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

        public ActionResult ShoppingCart()
        {
            List<Book> bookList = new List<Book>();
            if (Session["User_id"] != null)
            {
                int User_id = Convert.ToInt32(Session["User_id"]);
                var result = (from cart in db.Carts
                              from book in db.Books
                              from user in db.Users
                              where cart.ISBN == book.ISBN &&
                              user.User_id == User_id
                              select book).ToList();

                return View(result);
            }
            else
            {
                if (Session["shoppingCart"] != null)
                {
                    List<string> isbns = Session["shoppingCart"].ToString().Split(',').ToList();

                    try
                    {
                        foreach (string x in isbns)
                        {

                            double abc = Convert.ToDouble(x);
                            bookList.Add(db.Books.Where(m => m.ISBN == abc).FirstOrDefault());
                        }
                    }
                    catch (FormatException)
                    {

                    }


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