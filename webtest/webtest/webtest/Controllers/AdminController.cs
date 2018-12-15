using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webtest.Models;

namespace webtest.Controllers
{
    public class AdminController : Controller
    {
        DatabaseEntities1 db = new DatabaseEntities1();
        // GET: Admin
        public ActionResult Product(string ISBN = "", bool delete = false, bool add = false, bool change = false, bool edit = false, bool cancel = false, string Category = "", string Name = "", string Summary = "", string Date = "", string Author = "", string Image_src = "", string Price = "", string Rating = "", string Stock = "")
        {
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
                    var rating = Convert.ToInt32(Rating);
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
                        Rating = rating,
                        Stock = stock
                    };

                    db.Books.Add(bookNew);
                    db.SaveChanges();

                    Session["Admin_Book"] = null;
                    add = false;
                    return View();
                }
                else if (change == true)
                {
                    var date = Convert.ToDateTime(Date);
                    var price = Convert.ToDecimal(Price);
                    var rating = Convert.ToInt32(Rating);
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
                        x.Rating = rating;
                        x.Stock = stock;

                    }
                    db.SaveChanges();


                    Session["Edit_Book"] = null;
                    change = false;
                    Session["Admin_Book"] = null;
                    return View();
                }

                else
                {
                    if (edit == true)
                    {
                        Session["Edit_Book"] = true;
                        edit = false;
                    }

                    else if (cancel == true)
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
                Session["Admin_Book"] = null;
                return View();
            }
        }

        public ActionResult User(User userData, bool? Type, bool? EmailVerified, string User_id_search = "", string Email_search = "", string add = "", string Name = "", string Surname = "", string Email = "",
                    string Phone_number = "", string Password = "", string ConfirmPassword = "", bool delete = false, bool edit = false, bool cancel = false, bool change = false, string find = "")
        {


            if (add == "true")
            {

                if (ModelState.IsValid)
                {
                    using (var db = new DatabaseEntities1())
                    {
                        //CHECK of de email al bestaat in de database.
                        if (db.Users.Where(x => x.Email == userData.Email).ToList().Count == 0)
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
                            TempData["Success"] = "<script>lertify.success('Added to database !');</script>";
                        }
                        else
                        {
                            TempData["emailError"] = "<script>alertify.error('Email already exists');</script>";

                        }
                    }

                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);

                    foreach (var error in errors)
                    {
                        System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
                    }

                }
            }


            if (Email_search != "")
            {
                if (find == "true")
                {
                    Session["Edit_User"] = null;
                    change = false;
                }


                using (var db = new DatabaseEntities1())
                {
                    if (delete)
                    {
                        db.Users.Remove(db.Users.Where(x => x.Email == Email_search).FirstOrDefault());
                        db.SaveChanges();
                        Session["Edit_User"] = null;
                        change = false;
                        Session["Admin_User"] = null;
                        TempData["Success"] = "<script>alertify.success('Succesfully removed from database !');</script>";
                       
                        return View();
                    }
                    if (edit)
                    {
                        Session["Edit_User"] = Email_search;
                    }
                    else if (cancel)
                    {
                        Session["Edit_User"] = null;
                        cancel = false;
                    }
                    else if (change)
                    {
                        try
                        {
                            bool EmailVerifiedB = Convert.ToBoolean(EmailVerified);
                            bool TypeB = Convert.ToBoolean(Type);

                            User userChange = db.Users.Where(x => x.Email == Email_search).SingleOrDefault();
                            userChange.Name = Name;
                            userChange.Surname = Surname;
                            userChange.Email = Email;
                            userChange.Phone_number = Phone_number;
                            userChange.Password = userChange.Password;
                            userChange.ConfirmPassword = userChange.Password;
                            userChange.Type = TypeB;
                            userChange.IsEmailVerified = EmailVerifiedB;

                            db.SaveChanges();
                            Session["Edit_User"] = null;
                            change = false;
                            Session["Admin_User"] = null;
                            TempData["Success"] = "<script>alertify.success('Succesfully edited user !');</script>";

                        }
                        catch (Exception exception)
                        {

                        }


                    }

                    var User = db.Users.Where(x => x.Email == Email_search).FirstOrDefault();
                    Session["Admin_user"] = User;
                    if (User == null)
                    {
                        TempData["Success"] = "<script>alertify.error('User not found');</script>";
                    }

                    return View(User);


                }

            }








            Session["Admin_User"] = null;

            return View();

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
                statistcs[13] = Parenting ;
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
            foreach(Order order in db.Orders)
            {
                if (order.User_id == null)
                {
                    unregisterd = unregisterd + order.Payment.Amount;
                    Users[0] = unregisterd; 

                }

                if (order.User_id != 0 )
                {
                    registerd = registerd + order.Payment.Amount;
                    Users[1] = registerd;
                }

            }
            ViewBag.data = Users;
            return View();
        }
        public ActionResult DailySale(DateTime? date1, DateTime? date2)

        {

            var db = new DatabaseEntities1();
            int Sales1 = 0;
            int Sales2 = 0;
            int[] Sales = new int[2];
            foreach (Order order in db.Orders)
            {
                if (date1 == order.OrderDate)
                {
                    Sales1 = Sales1 + order.Payment.Amount;
                    Sales[0] = Sales1;
                }
                if (date2 == order.OrderDate)
                {
                    Sales2 = Sales2 + order.Payment.Amount;
                    Sales[1] = Sales2;
                }
                if (date1 == null)
                {
                    Sales1 = 0;
                    Sales[0] = Sales1;
                }
                if (date2 == null)
                {
                    Sales2 = 0;
                    Sales[1] = Sales2;
                }

            }
            ViewBag.data = Sales;
            return View();

        }
    }
}    

