using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webtest.Models;
using System.IO;

namespace webtest.Controllers
{
    public class AdminController : Controller
    {
        DatabaseEntities1 db = new DatabaseEntities1();
        // GET: Admin
        [HttpGet]
        public ActionResult Product(string ISBN = "", bool delete = false, bool add = false, bool addSession = false, bool change = false, bool edit = false, bool cancel = false, string Category = "", string Name = "", string Summary = "", string Date = "", string Author = "", string Image_src = "", string Price = "", decimal Rating = 0, string Stock = "")
        {
            if (addSession == true)
            {
                Session["Add_Book"] = true;
                Session["Admin_Book"] = null;
                return View();
            }

            if (ISBN != "")
            {
                var isbn = Convert.ToDouble(ISBN);

                if (delete == true)
                {
                    db.Books.Remove(db.Books.Where(m => m.ISBN == isbn).FirstOrDefault());
                    db.SaveChanges();

                    Session["Admin_Book"] = null;
                    delete = false;
                    return View();
                }
                else if (add == true)
                {
                    var date = Convert.ToDateTime(Date);
                    var price = Convert.ToDecimal(Price);
                    var stock = Convert.ToInt32(Stock);

                    var bookNew = new Book()
                    {
                        ISBN = isbn,
                        Category = Category,
                        Name = Name,
                        Summary = Summary,
                        Date = date,
                        Author = Author,
                        Image_src = Image_src,
                        Price = price,
                        Rating = Rating,
                        Stock = stock
                    };

                    db.Books.Add(bookNew);
                    db.SaveChanges();

                    Session["Admin_Book"] = null;
                    add = false;
                    return View();
                }

                else if (edit == true)
                {
                    Session["Edit_Book"] = true;
                    Session["Admin_Book"] = null;
                    edit = false;
                    return View(db.Books.Where(m => m.ISBN == isbn).FirstOrDefault());
                }

                else if (change == true)
                {
                    var date = Convert.ToDateTime(Date);
                    var price = Convert.ToDecimal(Price);
                    var stock = Convert.ToInt32(Stock);

                    var _change = (from book in db.Books
                                   where book.ISBN == isbn
                                   select book).ToList();

                    // Execute the query, and change the column values you want to change.
                    foreach (Book x in _change)
                    {

                        x.Category = Category;
                        x.Name = Name;
                        x.Summary = Summary;
                        x.Date = date;
                        x.Author = Author;
                        x.Image_src = Image_src;
                        x.Price = price;
                        x.Rating = Rating;
                        x.Stock = stock;

                    }
                    db.SaveChanges();


                    Session["Edit_Book"] = null;
                    change = false;
                    Session["Admin_Book"] = db.Books.Where(m => m.ISBN == isbn).FirstOrDefault();
                    return View(db.Books.Where(m => m.ISBN == isbn).FirstOrDefault());
                }

                else
                {
                    if (cancel == true)
                    {
                        Session["Edit_Book"] = null;
                        cancel = false;
                    }

                    Session["Admin_Book"] = db.Books.Where(m => m.ISBN == isbn).FirstOrDefault();
                    return View(db.Books.Where(m => m.ISBN == isbn).FirstOrDefault());
                }
            }
            else
            {
                if (cancel == true)
                {
                    Session["Add_Book"] = null;
                }
                Session["Admin_Book"] = null;
                return View();
            }
        }

        public ActionResult ProductList(String Find = "", bool delete = false, double ISBN = 0.0)
        {
            if (delete == true)
            {
                db.Books.Remove(db.Books.Where(m => m.ISBN == ISBN).FirstOrDefault());
                db.SaveChanges();

                delete = false;
                Session["Admin_BookList"] = null;
                return View();
            }

            // Find product by title and ISBN
            if (Find != "")
            {
                var select = db.Books.ToList();
                select = db.Books.Where(m => m.Name.Contains(Find) || m.ISBN.ToString().Contains(Find)).ToList();
                Session["Admin_BookList"] = select;

                // return table with all results
                return View(select);
            }

            else
            {
                Session["Admin_BookList"] = null;
                return View();
            }
        }

        public ActionResult UserList(String Find = "", bool delete = false, int User_id = 0)
        {
            if (delete == true)
            {
                db.Users.Remove(db.Users.Where(m => m.User_id == User_id).FirstOrDefault());
                db.SaveChanges();

                delete = false;
                Session["Admin_UserList"] = null;
                return View();
            }

            // Find product by title and ISBN
            if (Find != "")
            {
                var select = db.Users.ToList();
                select = db.Users.Where(m => m.Name.Contains(Find) || m.Email.Contains(Find)).ToList();
                Session["Admin_UserList"] = select;

                // return table with all results
                return View(select);
            }

            else
            {
                Session["Admin_UserList"] = null;
                return View();
            }
        }
        public ActionResult User(bool? Type, bool? EmailVerified, string User_id = "", bool delete = false, bool add = false, bool addSession = false, bool change = false,
            bool edit = false, bool cancel = false, string Name = "", string Surname = "", string Email = "",
                    string Phone_number = "", string Password = "", string ConfirmPassword = "")
        {
            if (addSession == true)
            {
                Session["Add_User"] = true;
                Session["Admin_User"] = null;
                return View();
            }

            if (User_id != "")
            {
                var user_id = Convert.ToInt32(User_id);

                if (delete == true)
                {
                    db.Users.Remove(db.Users.Where(x => x.User_id == user_id).FirstOrDefault());
                    db.SaveChanges();

                    Session["Admin_User"] = null;
                    delete = false;

                    TempData["Success"] = "<script>alertify.success('Succesfully deleted user !');</script>";
                    return View();
                }

                else if (edit == true)
                {
                    Session["Edit_User"] = true;
                    Session["Admin_User"] = null;
                    Session["Add_User"] = null;
                    edit = false;
                    return View(db.Users.Where(m => m.User_id == user_id).FirstOrDefault());
                }

                else if (change == true)
                {
                    try
                    {
                        bool EmailVerifiedB = Convert.ToBoolean(EmailVerified);
                        bool TypeB = Convert.ToBoolean(Type);

                        User userChange = db.Users.Where(x => x.User_id == user_id).SingleOrDefault();
                        userChange.Name = Name;
                        userChange.Surname = Surname;
                        userChange.Email = Email;
                        userChange.Phone_number = Phone_number;
                        userChange.Password = userChange.Password;
                        userChange.ConfirmPassword = userChange.Password;
                        userChange.Type = TypeB;
                        userChange.IsEmailVerified = EmailVerifiedB;

                        db.SaveChanges();

                        TempData["Success"] = "<script>alertify.success('Succesfully edited user !');</script>";

                    }
                    catch (Exception exception)
                    {
                        TempData["Success"] = "<script>alertify.success('Problem adding !');</script>";
                    }

                    Session["Edit_User"] = null;
                    change = false;
                    Session["Admin_User"] = db.Users.Where(m => m.User_id == user_id).FirstOrDefault();

                    return View(db.Users.Where(m => m.User_id == user_id).FirstOrDefault());

                }

                else
                {
                    if (cancel == true)
                    {
                        Session["Edit_User"] = null;
                        cancel = false;
                    }

                    Session["Admin_User"] = db.Users.Where(m => m.User_id == user_id).FirstOrDefault();
                    return View(db.Users.Where(m => m.User_id == user_id).FirstOrDefault());
                }
            }
            else if (add == true)
            {
                var User = db.Users.Where(x => x.Email == Email).ToList();

                if (User.Count == 0)
                {
                    bool EmailVerifiedB = Convert.ToBoolean(EmailVerified);
                    bool TypeB = Convert.ToBoolean(Type);
                    User addUser = new User()
                    {
                        Name = Name,
                        Surname = Surname,
                        Email = Email,
                        Password = Crypto.Hash(Password),
                        ConfirmPassword = Crypto.Hash(ConfirmPassword),
                        Phone_number = Phone_number,
                        Type = TypeB,
                        IsEmailVerified = EmailVerifiedB,
                        ActivationCode = Guid.NewGuid()
                    };

                    db.Users.Add(addUser);
                    db.SaveChanges();
                    TempData["Success"] = "<script>alertify.success('Added to database !');</script>";
                }
                else
                {
                    TempData["emailError"] = "<script>alertify.error('Email already exists');</script>";

                }

                Session["Admin_User"] = null;
                add = false;
                return View();
            }
            else
            {
                if (cancel == true)
                {
                    Session["Add_User"] = null;
                }
                Session["Admin_User"] = null;
                return View();
            }
        }

            public ActionResult CatagorieStatistics()
            {
            var db = new DatabaseEntities1();
            //var a = db.OrderDetails;
            //foreach (OrderDetail orderDetailObj in a)
            //{
            //    Dictionary<Book, int> BookQuantity = new Dictionary<Book, int>();

            //    {
            //        // GET PRODUCTS
            //        string[] products = orderDetailObj.Products.Split('|');

            //        foreach (var item in products)
            //        {
            //            string[] books = item.Split('-');
            //            double isbn = Convert.ToDouble(books[0]);
            //            Book book = db.Books.Where(x => x.ISBN == isbn).FirstOrDefault();
            //            int quantity = Convert.ToInt32(books[1]);
            //            BookQuantity.Add(book, quantity);
            //        }

            //}
            int Tottal = 0;
            foreach (Book prod1 in db.Books)
            {
                Tottal = Tottal + prod1.Stock;
            }


            int Fiction = 0;
            int Biography = 0;
            int Sports = 0;
            int Art = 0;
            int Science = 0;
            int Business = 0;
            int Education = 0;
            int Food = 0;
            int Style = 0;
            int Diet = 0;
            int History = 0;
            int Home = 0;
            int Mind = 0;
            int Parenting = 0;
            int[] statistcs = new int[14];


            foreach (Book prod1 in db.Books)
            {
                if (prod1.Category == "Fiction")
                {
                    int Fictionper = 0;
                    Fiction = Fiction + prod1.Stock;
                    statistcs[0] = Fiction;
                    Fictionper = (Fiction * 100) / Tottal;

                }

                if (prod1.Category == "Biography")
                {
                    Biography = Biography + prod1.Stock;
                    statistcs[1] = Biography;
                }
                if (prod1.Category == "Sports")
                {
                    Sports = Sports + prod1.Stock;
                    statistcs[2] = Sports;
                }
                if (prod1.Category == "Art & Photography")
                {
                    Art = Art + prod1.Stock;
                    statistcs[3] = Art;
                }
                if (prod1.Category == "Science & Nature")
                {
                    Science = Science + prod1.Stock;
                    statistcs[4] = Science;
                }
                if (prod1.Category == "Business")
                {
                    Business = Business + prod1.Stock;
                    statistcs[5] = Business;
                }
                if (prod1.Category == "Education")
                {
                    Education = Education + prod1.Stock;
                    statistcs[6] = Education;
                }
                if (prod1.Category == "Food & Drink")
                {
                    Food = Food + prod1.Stock;
                    statistcs[7] = Food;
                }
                if (prod1.Category == "Style & Beauty")
                {
                    Style = Style + prod1.Stock;
                    statistcs[8] = Style;
                }
                if (prod1.Category == "Diet & Fitness")
                {
                    Diet = Diet + prod1.Stock;
                    statistcs[9] = Diet;
                }
                if (prod1.Category == "History & Politics")
                {
                    History = History + prod1.Stock;
                    statistcs[10] = History;
                }
                if (prod1.Category == "Home & Garden")
                {
                    Home = Home + prod1.Stock;
                    statistcs[11] = Home;
                }
                if (prod1.Category == "Mind Body Spirit")
                {
                    Mind = Mind + prod1.Stock;
                    statistcs[12] = Mind;
                }
                if (prod1.Category == "Parenting")
                {
                    Parenting = Parenting + prod1.Stock;
                    statistcs[13] = Parenting;
                }

            }
            ViewBag.data = statistcs;
            return View();
        }
        public ActionResult UserOrderSta()
        {
            var db = new DatabaseEntities1();
            int[] Users = new int[2];
            int unregisterd = 0;
            int registerd = 0;
            foreach (Order order in db.Orders)
            {
                if (order.User_id == null)
                {
                    unregisterd = unregisterd + order.Payment.Amount;
                    Users[0] = unregisterd;

                }

                if (order.User_id != 0)
                {
                    registerd = registerd + order.Payment.Amount;
                    Users[1] = registerd;
                }

            }
            ViewBag.data = Users;
            return View();
        }

        public ActionResult DailySale(string Date1 = "01/01/2018", string Date2 = "01/01/2018")

        {
            var date1 = new DateTime();/*Convert.ToDateTime(Date1);*/
            var date2 = new DateTime();/*Convert.ToDateTime(Date2);*/
            if (Date1 == "" || Date2 == "")
            {
                date1 = Convert.ToDateTime("01/01/2018");
                date2 = Convert.ToDateTime("01/01/2018");
            }
            else
            {
                date1 = Convert.ToDateTime(Date1);
                date2 = Convert.ToDateTime(Date2);
            }
            var db = new DatabaseEntities1();
            int Sales1 = 0;
            int Sales2 = 0;
           
            int[] Sales = new int[8];
            Sales[2] = date1.Day;
            Sales[3] = date1.Month;
            Sales[4] = date1.Year;
            Sales[5] = date2.Day;
            Sales[6] = date2.Month;
            Sales[7] = date2.Year;
            foreach (Order order in db.Orders)
            {
                if (date1.Day == order.OrderDate.Day && date1.Month == order.OrderDate.Month)
                {
                    Sales1 = Sales1 + order.Payment.Amount;
                    Sales[0] = Sales1;
                }
                if (date2.Day == order.OrderDate.Day && date2.Month == order.OrderDate.Month)
                {
                    Sales2 = Sales2 + order.Payment.Amount;
                    Sales[1] = Sales2;
                }
                if (date1.Day == 0 && date1.Month == order.OrderDate.Month)
                {
                    Sales1 = Sales1 + order.Payment.Amount;
                    Sales[0] = Sales1;
                }
                if (date2.Day == 0 && date2.Month == order.OrderDate.Month)
                {
                    Sales2 = Sales2 + order.Payment.Amount;
                    Sales[1] = Sales2;
                }

                //if (date1 == null)
                //{
                //    Sales1 = 0;
                //    Sales[0] = Sales1;
                //}
                //if (date2 == null)
                //{
                //    Sales2 = 0;
                //    Sales[1] = Sales2;
                //}

            }
            ViewBag.data = Sales;
            return View();

        }
        public ActionResult MonthSales(string month1 , string month2, string year)
        {
            var db = new DatabaseEntities1();
           
            int Sales1 = 0;
            int Sales2 = 0;
            int[] Sales = new int[6];
            foreach (Order order in db.Orders)
            {
                if (month1 == "" || month2 == "")
                {
                    Sales1 = 10;
                    Sales2 = 10;
                }
                if (month1 == order.OrderDate.Month.ToString(format: "D2") && year == order.OrderDate.Year.ToString(format: "d4"))
                {
                    Sales1 = Sales1 + order.Payment.Amount;
                    Sales[0] = Sales1;
                    Sales[2] = order.OrderDate.Month;
                    Sales[3] = order.OrderDate.Year;
                }
                if (month2 == order.OrderDate.Month.ToString(format:"D2") && year == order.OrderDate.Year.ToString(format: "d4"))
                {
                    Sales2 = Sales2 + order.Payment.Amount;
                    Sales[1] = Sales2;
                    Sales[4] = order.OrderDate.Month;
                    Sales[5] = order.OrderDate.Year;
                }

            }
            ViewBag.data = Sales;
            return View();
        }
        public ActionResult YearlySales(string year)
        {

            var db = new DatabaseEntities1();
            int Year = Convert.ToInt32(year);
            int Sales1 = 0;
            int Sales2 = 0;
            int Sales3 = 0;
            int Sales4 = 0;
            int Sales5 = 0;
            int Sales6 = 0;
            int Sales7 = 0;
            int Sales8 = 0;
            int Sales9 = 0;
            int Sales10 = 0;
            int Sales11 = 0;
            int Sales12 = 0;

            int[] Sales = new int[13];
            foreach (Order order in db.Orders)
            {
                //if (year == "")
                //{
                //    Sales1 = 0;
                //}
                if (Year == order.OrderDate.Year && order.OrderDate.Month == 11)
                {
                    Sales1 = Sales1 + order.Payment.Amount;
                    Sales[10] = Sales1;
                }
                if (Year == order.OrderDate.Year && order.OrderDate.Month == 12)
                {
                    Sales2 = Sales2 + order.Payment.Amount;
                    Sales[11] = Sales2;
                }
                if (Year == order.OrderDate.Year && order.OrderDate.Month == 1)
                {
                    Sales1 = Sales1 + order.Payment.Amount;
                    Sales[0] = Sales1;
                }
                if (Year == order.OrderDate.Year && order.OrderDate.Month == 2)
                {
                    Sales2 = Sales2 + order.Payment.Amount;
                    Sales[1] = Sales2;
                }
                if (Year == order.OrderDate.Year && order.OrderDate.Month == 3)
                {
                    Sales3 = Sales3 + order.Payment.Amount;
                    Sales[2] = Sales3;
                }
                if (Year == order.OrderDate.Year && order.OrderDate.Month == 4)
                {
                    Sales4 = Sales4 + order.Payment.Amount;
                    Sales[3] = Sales1;
                }
                if (Year == order.OrderDate.Year && order.OrderDate.Month == 5)
                {
                    Sales5 = Sales5 + order.Payment.Amount;
                    Sales[4] = Sales5;
                }
                if (Year == order.OrderDate.Year && order.OrderDate.Month == 6)
                {
                    Sales6 = Sales6 + order.Payment.Amount;
                    Sales[5] = Sales6;
                }
                if (Year == order.OrderDate.Year && order.OrderDate.Month == 7)
                {
                    Sales7 = Sales7 + order.Payment.Amount;
                    Sales[6] = Sales7;
                }
                if (Year == order.OrderDate.Year && order.OrderDate.Month == 8)
                {
                    Sales8 = Sales8 + order.Payment.Amount;
                    Sales[7] = Sales8;
                }
                if (Year == order.OrderDate.Year && order.OrderDate.Month == 9)
                {
                    Sales9 = Sales9 + order.Payment.Amount;
                    Sales[8] = Sales9;
                }
                if (Year == order.OrderDate.Year && order.OrderDate.Month == 10)
                {
                    Sales10 = Sales10 + order.Payment.Amount;
                    Sales[9] = Sales10;
                }


            }
            ViewBag.data = Sales;
            return View();
        }

        DatabaseEntities1 obj = new DatabaseEntities1();
        //public ActionResult Product()
        //{
        //    return View();
        //}

        [HttpPost]
        public ActionResult Product(FormCollection fc, HttpPostedFileBase file)
        {
            Book book = new Book();
            var allowedExtensions = new[] {
            ".Jpg", ".png", ".jpg", "jpeg"
        };
            book.Image_src = file.ToString(); //hele url ophalen  
            book.Name = fc["Name"].ToString();
            var fileName = Path.GetFileName(file.FileName);
            var ext = Path.GetExtension(file.FileName);
            if (allowedExtensions.Contains(ext)) //bestandsextensie checken  
            {
                string name = Path.GetFileNameWithoutExtension(fileName); //bestandsextensie wordt eruit gelaten 
                string myfile = name + "_" + book.ISBN + ext; //myfile krijgt ISBN erbij 

                var path = Path.Combine(Server.MapPath("~/BookCover"), myfile); //bestand opslaan in projectmap BookCover
                book.Image_src = path;
                obj.Books.Add(book);
                obj.SaveChanges();
                file.SaveAs(path);
            }
            else
            {
                ViewBag.message = "Only an Image file can be chosen. No other type is allowed.";
            }
            return View();
        }

    }
}
  

