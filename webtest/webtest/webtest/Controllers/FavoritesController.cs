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
        public ActionResult Index(double? delete, double? cart)
        {
            if (Session["User_id"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
           
            int User_id = Convert.ToInt32(Session["User_id"]);
            var list = db.Favorites.Select(s => s);
            string deleteISBN = delete.ToString();
            string CartISBN = cart.ToString();


            // VERWIJDER UIT FAVORIETEN
            if (deleteISBN != "" && deleteISBN != null)
            {
                using (DatabaseEntities1 db = new DatabaseEntities1())
                {
                    db.Favorites.Remove(db.Favorites.Single(x => x.ISBN == delete && x.User_id == User_id));
                    db.SaveChanges();
                }
            }

            if (CartISBN != "" && CartISBN != null)
            {
                var listCart = db.Carts.Select(s => s);
                int User_idCart = Convert.ToInt32(Session["User_id"]);

                bool has = listCart.Any(x => x.ISBN == cart && x.User_id == User_id);

                if (has)
                {
                    using (DatabaseEntities1 db = new DatabaseEntities1())
                    {
                        db.Carts.Remove(db.Carts.Single(x => x.ISBN == cart && x.User_id == User_id));
                        db.SaveChanges();
                    }

                }
                else
                {
                    // ISBN TOEVOEGEN AAN FAVORIETEN

                    using (DatabaseEntities1 db = new DatabaseEntities1())
                    {
                        var cartObj = new Cart() { User_id = User_id, ISBN = Convert.ToDouble(cart), Quantity = 1 };
                        db.Carts.Add(cartObj);
                        db.SaveChanges();
                    }
                }
            }

            //VERWIJDEREN UIT WINKELWAGEN

            // laad favorieten
            var result = (from favo in db.Favorites
                          from book in db.Books
                          where favo.ISBN == book.ISBN &&
                          favo.User_id == User_id
                          select book).ToList();

            return View(result);
        }

    }
}