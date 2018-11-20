using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using webtest.Models;

namespace webtest.Controllers
{
    public class CheckoutController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        //LOGIN
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
                else if (userData.IsEmailVerified == false)
                {
                    userModel.LoginErrorMessage = "your email must be verified first before you can log in a new verification link has been sent tot your email ";
                    SendVerificationLinkEmail(userData.Email, userData.ActivationCode.ToString());
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

        //CHECKOUT AS GUEST
        public ActionResult Address()
        {
            return View();
        }

        public ActionResult Payment()
        {
            return View();
        }

        public ActionResult Review()
        {
            return View();
        }

        public ActionResult Confirmation()
        {
            return View();
        }




        [NonAction]
        public void SendVerificationLinkEmail(string email, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Register/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("bookus094@gmail.com", "BookStore");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "Hogeschool_rotterdam1";

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created";

                body = "<br/><br/> Welcome by bookstore your account has been" +
                   " successfuly created. Please click on the link below to verify your account" +
                   " <br/><br/><a herf='" + link + "'>" + link + "</a> ";
            }

            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hallo,<br/><br/>Dear User of BookRus we just have got a request to reset your password. If you want realy to reset your password, you can click on the link below" +
                       "<br/><br/><a herf='" + link + "'>" + link + "</a>";
            }
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

    
    }
}