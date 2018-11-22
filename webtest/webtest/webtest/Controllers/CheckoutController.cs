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
                    //SUCCESFULLY LOGIN
                    Session["User_id"] = userData.User_id;
                    Session["Name"] = userData.Name;

                    var cart = (Dictionary<Book, int>)Session["Cart"];

                    db.Carts.RemoveRange(db.Carts.Where(x => x.User_id == userData.User_id));
                    db.SaveChanges();
                    foreach (KeyValuePair<Book, int> kv in cart)
                    {
                        var cartObj = new Cart() { User_id = userData.User_id, ISBN = kv.Key.ISBN, Quantity = kv.Value };
                        db.Carts.Add(cartObj);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Address", "Checkout");

                }
            }
        }

        //CHECKOUT AS GUEST

        public ActionResult Address(string Name, string LastName, string Email, string PhoneNumber, string Street,
            int? HouseNumber, string City, string ZipCode, string Country, string Save)
        {
            Dictionary<User, Address> userInfo = new Dictionary<User, Address>();


            if (Name == "" || LastName == "" || Email == "" || PhoneNumber == "" || Street == "" ||
                HouseNumber == null || City == "" || ZipCode == "" || Country == "")
            {
                ViewBag.Message = "Please fill in all info";
                return View();
            }
            else
            {
                if (Session["User_id"] != null)
                {
                    int User_id = Convert.ToInt32(Session["User_id"]);

                    userInfo.Add(new User { Name = Name, Surname = LastName, Email = Email, Phone_number = PhoneNumber },
                       new Address
                       {
                           Street_name = Street,
                           House_number = Convert.ToInt32(HouseNumber),
                           Zip_code = ZipCode,
                           Country = Country,
                           User_id = User_id
                       });

                    Session["UserInfo"] = userInfo;

                    if (Save == "Save")
                    {

                        using (var db = new DatabaseEntities1())
                        {
                            var has = db.Addresses.Where(x => x.User_id == User_id).SingleOrDefault();
                            if (has != null)
                            {
                                db.Addresses.Remove(db.Addresses.Where(x => x.User_id == User_id).SingleOrDefault());
                            }


                            db.Addresses.Add(new Address
                            {
                                Street_name = Street,
                                City = City,
                                House_number = Convert.ToInt32(HouseNumber),
                                Zip_code = ZipCode,
                                Country = Country,
                                User_id = User_id
                            });
                            db.SaveChanges();


                        }
                    }

                    return RedirectToAction("Review");
                }
                else
                {
                    userInfo.Add(new User { Name = Name, Surname = LastName, Email = Email, Phone_number = PhoneNumber },
                       new Address
                       {
                           Street_name = Street,
                           House_number = Convert.ToInt32(HouseNumber),
                           Zip_code = ZipCode,
                           Country = Country
                       });

                    Session["UserInfo"] = userInfo;

                    return RedirectToAction("Review");
                }
                   
            }


        }


        public ActionResult Payment(bool? payment)
        {
            if(payment != null)
            {
                if ((bool)payment)
                {

                    
                    string products = "";

                    if (Session["User_id"] != null)
                    {

                    }
                    else
                    {
                        Dictionary<Book, int> cartQuantity = (Dictionary<Book, int>)Session["Cart"];
                        foreach (KeyValuePair<Book, int> kv in cartQuantity)
                            {
                                products += kv.Key.ISBN.ToString() + "-" + kv.Value + "|";
                            }
                    }
                    


                    int Ordernumber = new Random().Next(1000000, int.MaxValue);


                    using (var db = new DatabaseEntities1())
                    {
                        while (db.Orders.Where(x => x.Order_Number == Ordernumber).FirstOrDefault() != null)
                        {
                            Ordernumber = new Random().Next(1000000, int.MaxValue);
                        }
                        var has = db.Orders.Where(x => x.Order_Number == Ordernumber).FirstOrDefault();
                        if (has == null)
                        {
                            if (Session["User_id"] != null)
                            {
                                Order OrderObj = new Order()
                                {
                                    Order_status = "Pending",
                                    OrderDate = DateTime.Now,
                                    User_id = Convert.ToInt32(Session["User_id"]),
                                    Order_Number = Ordernumber
                                };

                            }
                            else
                            {
                                Order OrderObj = new Order()
                                {
                                    Order_status = "Pending",
                                    OrderDate = DateTime.Now,
                                    Order_Number = Ordernumber
                                };

                            }

                        }

                    }

                }
            }
            
            return View();
        }

        public ActionResult Review()
        {
            Dictionary<User, Address> userInfo = (Dictionary < User, Address > )Session["UserInfo"];
            return View(userInfo);
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