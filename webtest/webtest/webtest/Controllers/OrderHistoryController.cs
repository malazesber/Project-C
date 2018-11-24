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

            int User_id = Convert.ToInt32(Session["User_id"]);
            var list = db.Orders.Select(s => s);
           




            // laad orders
            var result = (from order in db.Orders
                          from user in db.Users
                          where user.User_id == User_id
                          select order).ToList();

            return View(result);
        }
        public ActionResult OrderDetails(OrderDetail orederdetail)
        {
            var result = (from orderdetails in db.OrderDetails
                          from order in db.Orders
                          where order.OrderDetails_id == orderdetails.OrderDetails_Id
                          select orderdetails).ToList();

            return View(result);
        }

    }
}