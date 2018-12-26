using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webtest.Models;


namespace webtest.Controllers
{
    public class OrderHistoryController : Controller
    {
        DatabaseEntities1 db = new DatabaseEntities1();
        // GET: Orders
        public ActionResult OrderHistory()
        {
            if (Session["User_id"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            
            else

            {

                int User_id = Convert.ToInt32(Session["User_id"]);
                





                // laad orders
                var result = (from order in db.Orders
                              where order.User_id == User_id
                              orderby order.OrderDate descending
                              select order).ToList();


                return View(result);
            }
        }
        public ActionResult OrderDetails (int id)
        {

            Dictionary<Book, int> BookQuantity = new Dictionary<Book, int>();
            Tuple<Order, OrderDetail, Payment> info;
            using (var db = new DatabaseEntities1())
            {
                Order orderObj = db.Orders.Where(x => x.Order_Number == id).FirstOrDefault();
                OrderDetail orderDetailObj = db.OrderDetails.Where(x => x.Order_Number == id).FirstOrDefault();
                Payment paymentObj = db.Payments.Where(x => x.Order_Number == id).FirstOrDefault();

                // GET PRODUCTS
                string[] products = orderDetailObj.Products.Split('|');

                foreach (var item in products)
                {
                    string[] books = item.Split('-');
                    double isbn = Convert.ToDouble(books[0]);
                    Book book = db.Books.Where(x => x.ISBN == isbn).FirstOrDefault();
                    int quantity = Convert.ToInt32(books[1]);
                    BookQuantity.Add(book, quantity);


                }

                info = new Tuple<Order, OrderDetail, Payment>(orderObj, orderDetailObj, paymentObj);

                Session["Checkout"] = BookQuantity;
            }
            return View(info);
        }

    }
}