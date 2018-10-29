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
    public class RegisterController : Controller
    {
        //Registration Action
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }
        //Registration Post Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude ="IsEmailVerified,ActivationCode,Type,User_id")]User user)
        {
            bool status = false;
            string message = "";
            //
            //Model Validation
            if (ModelState.IsValid)
            {
                //Email is already Exist
                var isExist = IsEmailExist(user.Email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(user);
                }
                //Generate Activation Code
                user.ActivationCode = Guid.NewGuid();
                //Password Hashing
                user.Password = Crypto.Hash(user.Password);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);

                user.IsEmailVerified = false;

                //Save data to Database
                using (DatabaseEntities1 dc = new DatabaseEntities1())
                {
                    dc.Users.Add(user);
                    dc.SaveChanges();
                    //Send Email to User
                    SendVerificationLinkEmail(user.Email, user.ActivationCode.ToString());
                    message = "Registration successfully done. An activation link " +
                        " has been sent to your email:" + user.Email;
                    status = true;
                }

            }
            else
            {
                message = "Invalid Request";
            }







            ViewBag.Message = message;
            ViewBag.Status = status;
            return View(user);
        }
        //Verify Email

        [HttpGet]
        public ActionResult VerifyAccount (string id)
        {
            bool status = false;
            using (DatabaseEntities1 dc = new DatabaseEntities1())
            {
                dc.Configuration.ValidateOnSaveEnabled = false; //This line will avoid any problems by confirm password 
                                                                //does not match issue in the save changes section
                var v = dc.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    status = true;
                }
                else
                {
                    ViewBag.Message = "Invaild Requist";
                }
            }
            ViewBag.Status = status;
            return View();
        }
        

        [NonAction]
        public bool IsEmailExist(string email)
        {
            using (DatabaseEntities1 dc = new DatabaseEntities1())
            {
                var v = dc.Users.Where(a => a.Email == email).FirstOrDefault();
                return v != null;
            }

        }

        //this fuction will be used to send the verifiction password and the rest link
        //it can be used also to send email for the user.
        //you have to provide parameters for the function.
        [NonAction]
        public void SendVerificationLinkEmail(string email, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Register/"+emailFor+"/" + activationCode;
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
                   " <br/><br/><a herf='"+link+"'>"+link+"</a> ";
            }

            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hallo,<br/><br/>Dear User of BookRus we just have got a request to reset your password. If you want realy to reset your password, you can click on the link below"+
                       "<br/><br/><a herf='"+link+"'>"+link+"</a>";
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

        //Forgot Password

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            //verify Email
            //GEnerate Reser Password link
            //Send Email
            string message = "";
            bool status = false;
            using (DatabaseEntities1 dc = new DatabaseEntities1())
            {
                var account = dc.Users.Where(a => a.Email == Email).FirstOrDefault();
                if (account != null)
                {
                    //Send reset password email
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.Email, resetCode, "ResetPassword");
                    account.ResetPasswordCode = resetCode;
                    //

                    //this code wil prevent the website from having issues with password do not match
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();
                    message = "A link to reset your password has been sent to your Email.";
                
                }
                else
                {
                    message = "Account not found";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            using (DatabaseEntities1 dc = new DatabaseEntities1())
            {
                var user = dc.Users.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel
                    {
                        ResetCode = id
                    };
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (DatabaseEntities1 dc = new DatabaseEntities1())
                {
                    var user = dc.Users.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        user.Password = Crypto.Hash(model.NewPassword);
                        user.ResetPasswordCode = "";
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();
                        message = "Your Password has been successfully updated";
                    }
                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }
    }

    
}