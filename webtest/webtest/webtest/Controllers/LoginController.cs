﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
                var Data = db.Users.Where(x => x.Email == userModel.Email
                 && x.Password == userPassword).FirstOrDefault();

                if(userData.Type)
                {
                    Session["Admin"] = true;
                }
                else
                {
                    Session["Admin"] = false;
                }

                if (userData == null)
                {
                    userModel.LoginErrorMessage = "Wrong Email or Password";
                    return View("Index", userModel);
                }
                else if (Data.IsEmailVerified == false)
                {
                    userModel.LoginErrorMessage = "Your email must be verified before you can log in. A new verification link has been sent to your email ";
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

                body = "<br/><br/> Welcome by BooksR'us your account has been" +
                   " successfuly created. Please click on the link below to verify your account" +
                   " <br/><br/><a herf='" + link + "'>" + link + "</a> ";
            }

            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hallo,<br/><br/>Dear User of BooksR'us, we have received a request to reset your password. If you really want to reset your password, you can click on the link below" +
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
            Session["User_id"] = null;
            Session["ReviewInput"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}