using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webtest.Models;

namespace webtest.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authorize(User userModel)
        {
            using (DatabaseEntities1 db = new DatabaseEntities1())
            {
                var userPassword = Crypto.Hash(userModel.Password);
                var userData = db.Users.Where(x => x.Email == userModel.Email
                 && x.Password == userPassword).FirstOrDefault();
                if (userData == null)
                {
                    
                    userModel.LoginErrorMessage = "Wrong Email or Password";
                    return View("Index", userModel);
                }
                else
                {
                    Session["User_id"] = userData.User_id;
                    Session["Name"] = userData.Name;
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}