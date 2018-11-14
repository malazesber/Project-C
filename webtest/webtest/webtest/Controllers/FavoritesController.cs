using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webtest.Models;

namespace webtest.Controllers
{
    public class FavoritesController : Controller
    {
        DatabaseEntities1 db = new DatabaseEntities1();
        // GET: Favorites
        public ActionResult Index()
        {
            ////Selecteer alle boeken en favorieten
            //var listFavo = db.Favorites.Select(s => s);
            //var listBooks = db.Books.Select(s => s);

            ////filter favorieten op user id
            int User_id = Convert.ToInt32(Session["User_id"]);
            //var select = listFavo.Where(x => x.User_id == User_id).ToList();

            var result = (from favo in db.Favorites
                          from book in db.Books
                          from user in db.Users
                          where favo.ISBN == book.ISBN &&
                          user.User_id == User_id
                          select book).ToList();

            return View(result);
        }

    }
}