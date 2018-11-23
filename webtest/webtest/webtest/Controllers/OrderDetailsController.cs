using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webtest.Models;

namespace webtest.Controllers
{
    public class OrderDetailsController : Controller

    {
        DatabaseEntities1 db = new DatabaseEntities1();
        // GET: OrderDetails
        public ActionResult OrderDetails()
        {
            decimal totalPrice = 0;
            List<Book> bookList = new List<Book>();
            List<Book> bookReturn = new List<Book>();
            Dictionary<Book, int> cartQuantity = new Dictionary<Book, int>();

            if (Session["User_id"] != null)
            {
                int User_id = Convert.ToInt32(Session["User_id"]);


                var result = (from OrderDetail in db.OrderDetails
                              from book in db.Books
                              from user in db.Users
                              where user.User_id == User_id
                              select book).ToList();


                foreach (var i in result)
                {
                    var item = db.Carts.Where(x => x.ISBN == i.ISBN).FirstOrDefault();
                    for (int j = 0; j < item.Quantity; j++)
                    {
                        totalPrice += i.Price;
                    }

                }

                ViewBag.totalPrice = totalPrice;
                return View(result);

            }
        }
    }
}