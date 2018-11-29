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
                    userModel.LoginErrorMessage = "Your email must be verified before you can log in. A new verification link has been sent to your email ";
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
                ViewBag.Message = "Please fill in everything.";
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
            if (payment != null)
            {
                if ((bool)payment)
                {

                    string products = "";
                    Dictionary<User, Address> userInfo = (Dictionary<User, Address>)Session["UserInfo"];

                    int Ordernumber = new Random().Next(1000000, int.MaxValue);


                    using (var db = new DatabaseEntities1())
                    {
                        //Check if ordernumber already exists.
                        while (db.Orders.Where(x => x.Order_Number == Ordernumber).FirstOrDefault() != null)
                        {
                            Ordernumber = new Random().Next(1000000, int.MaxValue);
                        }
                        // CREATE ORDER IN DATABASE
                        Order orderObj = new Order();

                        if (Session["User_id"] != null)
                        {
                            //CREATER ORDER OBJECT
                            orderObj = new Order()
                            {
                                Order_status = "Pending",
                                OrderDate = DateTime.Now,
                                User_id = Convert.ToInt32(Session["User_id"]),
                                Order_Number = Ordernumber
                            };
                            //ADD ORDER TO DATABASE
                            db.Orders.Add(orderObj);
                            db.SaveChanges();

                            //READ ALL PRODUCTS AND QUANITIES TO STRING
                            Dictionary<Book, int> bookDict = new Dictionary<Book, int>();
                            var User_id = Convert.ToInt32(Session["User_id"]);


                            var a = db.Carts.Where(x => x.User_id == User_id).ToList();
                            foreach (var item in a)
                            {
                                Book bookObj = db.Books.Where(x => x.ISBN == item.ISBN).FirstOrDefault();
                                bookDict.Add(bookObj, item.Quantity);
                            }

                            int maxCount = bookDict.Count;
                            int counter = 0;
                            foreach (KeyValuePair<Book, int> kv in bookDict)
                            {
                                counter += 1;
                                if (counter == maxCount)
                                {
                                    products += kv.Key.ISBN.ToString() + "-" + kv.Value;
                                }
                                else
                                {
                                    products += kv.Key.ISBN.ToString() + "-" + kv.Value + "|";
                                }

                            }

                            //ADD ORDERDETAILS TO DB
                            foreach (KeyValuePair<User, Address> kv2 in userInfo)
                            {
                                OrderDetail orderDetailObj = new OrderDetail()
                                {
                                    OrderDate = DateTime.Now,
                                    Products = products,
                                    Name = kv2.Key.Name,
                                    Surname = kv2.Key.Surname,
                                    Email = kv2.Key.Email,
                                    Phone_Number = kv2.Key.Phone_number,
                                    Street_Name = kv2.Value.Street_name,
                                    House_number = kv2.Value.House_number,
                                    Zip_code = kv2.Value.Zip_code,
                                    Country = kv2.Value.Country,
                                    Order_Number = Ordernumber
                                };
                                //CREATE PAYMENT OBJ AND ADD IT TO PAYMENT TABLE
                                Payment PaymentObj = new Payment
                                {
                                    Payment_date = DateTime.Now,
                                    Amount = Convert.ToInt32(Session["TotalPrice"]),
                                    Order_Number = Ordernumber
                                };

                                db.OrderDetails.Add(orderDetailObj);
                                db.Payments.Add(PaymentObj);
                                db.SaveChanges();
                            }
                            //ADD ORDERDETAIL ID & PAMYENT ID TO ORDER
                            var orderDetailObj2 = db.OrderDetails.Where(x => x.Order_Number == Ordernumber).FirstOrDefault();
                            var paymentObj2 = db.Payments.Where(x => x.Order_Number == Ordernumber).FirstOrDefault();

                            var orderObj2 = db.Orders.Where(x => x.Order_Number == Ordernumber).FirstOrDefault();
                            orderObj2.OrderDetails_id = orderDetailObj2.OrderDetails_Id;
                            orderObj2.Payment_id = paymentObj2.Payment_id;




                            db.SaveChanges();

                            return RedirectToAction("Confirmation", new { orderNumber = Ordernumber });
                        }
                        else
                        {
                            // CREATE ORDER OBJECT
                            orderObj = new Order()
                            {
                                Order_status = "Pending",
                                OrderDate = DateTime.Now,
                                Order_Number = Ordernumber
                            };
                            //ADD ORDER TO DATABASE
                            db.Orders.Add(orderObj);
                            db.SaveChanges();

                            //READ ALL PRODUCTS AND QUANITIES TO STRING
                            Dictionary<Book, int> cartQuantity = (Dictionary<Book, int>)Session["Cart"];
                            int maxCount = cartQuantity.Count;
                            int counter = 0;
                            foreach (KeyValuePair<Book, int> kv in cartQuantity)
                            {
                                counter += 1;
                                if (counter == maxCount)
                                {
                                    products += kv.Key.ISBN.ToString() + "-" + kv.Value;
                                }
                                else
                                {
                                    products += kv.Key.ISBN.ToString() + "-" + kv.Value + "|";
                                }
                            }

                            // ADD ORDERDETAILS TO DB
                            foreach (KeyValuePair<User, Address> kv2 in userInfo)
                            {
                                //CREATE ORDERDETAIL OBJ AND ADD IT TO ORDERDETAIL TABLE
                                OrderDetail orderDetailObj = new OrderDetail()
                                {
                                    OrderDate = DateTime.Now,
                                    Products = products,
                                    Name = kv2.Key.Name,
                                    Surname = kv2.Key.Surname,
                                    Email = kv2.Key.Email,
                                    Phone_Number = kv2.Key.Phone_number,
                                    Street_Name = kv2.Value.Street_name,
                                    House_number = kv2.Value.House_number,
                                    Zip_code = kv2.Value.Zip_code,
                                    Country = kv2.Value.Country,
                                    Order_Number = Ordernumber
                                };

                                //CREATE PAYMENT OBJ AND ADD IT TO PAYMENT TABLE
                                Payment PaymentObj = new Payment
                                {
                                    Payment_date = DateTime.Now,
                                    Amount = Convert.ToInt32(Session["TotalPrice"]),
                                    Order_Number = Ordernumber

                                };

                                db.OrderDetails.Add(orderDetailObj);
                                db.Payments.Add(PaymentObj);
                                db.SaveChanges();
                            }

                            //ADD ORDERDETAIL ID TO ORDER
                            var orderDetailObj2 = db.OrderDetails.Where(x => x.Order_Number == Ordernumber).FirstOrDefault();
                            var paymentObj2 = db.Payments.Where(x => x.Order_Number == Ordernumber).FirstOrDefault();

                            var orderObj2 = db.Orders.Where(x => x.Order_Number == Ordernumber).FirstOrDefault();
                            orderObj2.OrderDetails_id = orderDetailObj2.OrderDetails_Id;
                            orderObj2.Payment_id = paymentObj2.Payment_id;
                            db.SaveChanges();


                            return RedirectToAction("Confirmation", new { orderNumber = Ordernumber });
                        }


                    }

                }
            }

            return View();
        }

        public ActionResult Review()
        {
            Dictionary<User, Address> userInfo = (Dictionary<User, Address>)Session["UserInfo"];
            return View(userInfo);
        }

        public ActionResult Confirmation(int orderNumber)
        {

            Dictionary<Book, int> BookQuantity = new Dictionary<Book, int>();
            Tuple<Order, OrderDetail, Payment> info;
            using (var db = new DatabaseEntities1())
            {
                Order orderObj = db.Orders.Where(x => x.Order_Number == orderNumber).FirstOrDefault();
                OrderDetail orderDetailObj = db.OrderDetails.Where(x => x.Order_Number == orderNumber).FirstOrDefault();
                Payment paymentObj = db.Payments.Where(x => x.Order_Number == orderNumber).FirstOrDefault();



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

                // CLEAR CARTS
                if(Session["User_id"] != null)
                {
                    int User_id = Convert.ToInt32(Session["User_id"]);
                    db.Carts.RemoveRange(db.Carts.Where(x => x.User_id == User_id));
                    db.SaveChanges();
                }
                else
                {
                    Session["ShoppingCart"] = null;
                }
            }
            return View(info);
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

                body = "<br/><br/> Welcome to BookR'us your account has been" +
                   " successfuly created. Please click on the link below to verify your account" +
                   " <br/><br/><a herf='" + link + "'>" + link + "</a> ";
            }

            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hallo,<br/><br/>Dear User of BookR'us, we have received a request to reset your password. If you really want to reset your password, you can click on the link below." +
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