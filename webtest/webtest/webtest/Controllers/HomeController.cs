﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webtest.Models;

namespace webtest.Controllers
{
    public class HomeController : Controller
    {
        //Connectie met database
        DatabaseEntities1 db = new DatabaseEntities1();

        public ActionResult Index(string Title, double? isbn, double? favo, double? cart)
        {
            string favoISBN = favo.ToString();
            string cartISBN = cart.ToString();
            var _isbn = isbn.ToString();

            if (favoISBN != "" && favoISBN != null)
            {
                if (Session["User_id"] == null)
                {
                    TempData["favo"] = "<script>alert('You need to login first.');</script>";

                }
                else
                {
                    var list = db.Favorites.Select(s => s);
                    double isbnD = Convert.ToDouble(isbn);
                    int User_id = Convert.ToInt32(Session["User_id"]);

                    bool has = list.Any(cus => cus.ISBN == isbnD && cus.User_id == User_id);
                    //CHECKEN OF ISBN AL IN FAVORIETEN ZIT VAN DE GEBRUIKER.
                    if (has)
                    {
                        using (DatabaseEntities1 db = new DatabaseEntities1())
                        {
                            db.Favorites.Remove(db.Favorites.Single(cus => cus.ISBN == isbnD && cus.User_id == User_id));
                            db.SaveChanges();
                        }

                    }
                    else
                    {
                        // ISBN TOEVOEGEN AAN FAVORIETEN

                        using (DatabaseEntities1 db = new DatabaseEntities1())
                        {
                            var favo1 = new Favorite() { User_id = User_id, ISBN = Convert.ToDouble(isbn) };
                            db.Favorites.Add(favo1);
                            db.SaveChanges();
                        }
                    }
                }
            }

            //Cart button toevoegen
            if (cartISBN != "" && cartISBN != null)
            {
                if (Session["User_id"] != null)
                {
                    var list = db.Carts.Select(s => s);
                    double isbnD = Convert.ToDouble(isbn);
                    int User_id = Convert.ToInt32(Session["User_id"]);

                    bool has = list.Any(cus => cus.ISBN == isbnD && cus.User_id == User_id);

                    if (has)
                    {
                        using (DatabaseEntities1 db = new DatabaseEntities1())
                        {
                            db.Carts.Remove(db.Carts.Single(cus => cus.ISBN == isbnD && cus.User_id == User_id));
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        using (DatabaseEntities1 db = new DatabaseEntities1())
                        {
                            var cart1 = new Cart() { User_id = User_id, ISBN = Convert.ToDouble(isbn), Quantity = 1 };
                            db.Carts.Add(cart1);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    // UNREGISTERED USER
                    if (Session["shoppingCart"] == null || Session["shoppingCart"] == "")
                    {
                        Session["shoppingCart"] = isbn.ToString();
                    }
                    else
                    {
                        List<string> isbns = Session["shoppingCart"].ToString().Split(',').ToList();
                        //Check of die al in je cart zit.
                        //Deletes product from cart
                        if (isbns.Contains(_isbn))
                        {
                            isbns.RemoveAll(s => _isbn == s);
                            var newcart = String.Join(",", isbns);
                            Session["shoppingCart"] = newcart;
                        }
                        else
                        {
                            Session["shoppingCart"] = Session["shoppingCart"] + "," + isbn.ToString();
                        }
                    }
                }
            }

            return View();
        }
        public ActionResult ShoppingCart(string isbn, double? del, double? favo, bool delete = false, bool plus = false, bool min = false)
        {
            decimal totalPrice = 0;
            string deleteISBN = del.ToString();
            List<Book> bookList = new List<Book>();
            List<Book> bookReturn = new List<Book>();
            Dictionary<Book, int> cartQuantity = new Dictionary<Book, int>();

            string favoISBN = favo.ToString();

            if (favoISBN != "" && favoISBN != null)
            {

                var list = db.Favorites.Select(s => s);
                int User_id = Convert.ToInt32(Session["User_id"]);

                bool has = list.Any(cus => cus.ISBN == favo && cus.User_id == User_id);
                //CHECKEN OF ISBN AL IN FAVORIETEN ZIT VAN DE GEBRUIKER.
                if (has)
                {
                    using (DatabaseEntities1 db = new DatabaseEntities1())
                    {
                        db.Favorites.Remove(db.Favorites.Single(cus => cus.ISBN == favo && cus.User_id == User_id));
                        db.SaveChanges();
                    }

                }
                else
                {
                    // ISBN TOEVOEGEN AAN FAVORIETEN

                    using (DatabaseEntities1 db = new DatabaseEntities1())
                    {
                        var favoObj = new Favorite() { User_id = User_id, ISBN = Convert.ToDouble(favo) };
                        db.Favorites.Add(favoObj);
                        db.SaveChanges();
                    }
                }

            }

            if (Session["User_id"] != null)
            {
                int User_id = Convert.ToInt32(Session["User_id"]);

                //Verwijder item voor geregistreerde user
                if (deleteISBN != "" && deleteISBN != null)
                {
                    using (DatabaseEntities1 db = new DatabaseEntities1())
                    {
                        db.Carts.Remove(db.Carts.Single(x => x.ISBN == del));
                        db.SaveChanges();
                    }
                }

                if (plus)
                {
                    /*  ValidISBN has to be made to succesfully do the Where() part of the query for Book.
                     *  book fetches the specific item of the shopping cart that has been selected, which holds the data of the book and the quantity in which it is present.
                     *  BookStock fetches the Stock data from the database Where the isbn is the same as the book that has been selected.
                     */ 
                    double ValidISBN = Convert.ToDouble(isbn);
                    var book = db.Carts.Where(x => x.ISBN == ValidISBN).FirstOrDefault();
                    int BookStock = db.Books.Where(x => x.ISBN == book.ISBN).FirstOrDefault().Stock;
                    if (book.Quantity >= BookStock)
                    {
                        //Essentially nothing happens, except for resetting the plus value.
                        plus = false;
                        ViewBag.Error = "We only have " + BookStock.ToString() + " copies in stock. You have reached the maximum amount.";
                    }
                    else
                    {
                        //Adds 1 to the Quantity of the selected book in the shopping cart and saves it in the database.
                        book.Quantity += 1;
                        db.SaveChanges();
                        plus = false;
                        ViewBag.Error = "";
                    }
                }

                if (min)
                {
                    double isbnD = Convert.ToDouble(isbn);
                    var book = db.Carts.Where(x => x.ISBN == isbnD).FirstOrDefault();
                    if(book.Quantity == 1)
                    {
                        using (DatabaseEntities1 db = new DatabaseEntities1())
                        {
                            db.Carts.Remove(db.Carts.Single(x => x.ISBN == isbnD));
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        book.Quantity -= 1;
                    }
                    
                    db.SaveChanges();
                }

                var result = (from cart in db.Carts
                              from book in db.Books
                              where cart.ISBN == book.ISBN &&
                              cart.User_id == User_id
                              select book).ToList();

                foreach (var i in result)
                {
                    var item = db.Carts.Where(x => x.ISBN == i.ISBN).FirstOrDefault();
                    for(int j = 0; j < item.Quantity; j++)
                    {
                        totalPrice += i.Price;
                    } 
                }

                ViewBag.totalPrice = totalPrice;
                return View(result);
            }
        
            else
            {
                if (Session["shoppingCart"] != null)
                {
                    List<string> isbns = Session["shoppingCart"].ToString().Split(',').ToList();
                    var bookListQ = from x in isbns
                            group x by x into g
                            let count = g.Count()
                            
                            select new { Value = g.Key, Count = count };

                    var total = 0;

                    //Deletes product from cart
                    if (isbns.Contains(isbn) && delete)
                    {
                        isbns.RemoveAll(s => isbn == s);
                        var newcart = String.Join(",", isbns);
                        Session["shoppingCart"] = newcart;
                        delete = false;
                    }

                    if (plus)
                    {
                     /* ValidISBN has to be made to succesfully do the Where() part of the query for Book.
                     *  BookStock fetches the Stock data from the database Where the isbn is the same as the book that has been selected.
                     *  Quantity fetches the Count() in which the selected book is present in the Shopping Cart. 
                     *  The items in the shopping cart are presented by their ISBN in isbns. (a List<string>).
                     */
                        double ValidISBN = Convert.ToDouble(isbn);
                        int BookStock = db.Books.Where(x => x.ISBN == ValidISBN).FirstOrDefault().Stock;
                        int Quantity = isbns.Where(x => x == isbn).Count();

                        if (Quantity < BookStock)
                        {
                            //Adds 1 to the Quantity of the selected book in the shopping cart and saves it in the Session["shoppingCart"] and isbns.
                            isbns.Add(isbn);
                            var newcart = String.Join(",", isbns);
                            Session["shoppingCart"] = newcart;
               
                            plus = false;
                            ViewBag.Error = "";
                        }
                        else
                        {
                            //Essentially nothing happens, except for resetting the plus value.
                            plus = false;

                            ViewBag.Error = "We only have " + BookStock.ToString() + " copies in stock. You have reached the maximum amount.";
                        }
                    }

                    if (min)
                    {
                        isbns.Remove(isbn);
                        var newcart = String.Join(",", isbns);
                        Session["shoppingCart"] = newcart;

                        min = false;
                    }

                    try
                    {
                        foreach(var book in bookListQ)
                        {
                            double bookISBN = Convert.ToDouble(book.Value);
                            int count = Convert.ToInt32(book.Count);
                            bookList.Add(db.Books.Where(m => m.ISBN == bookISBN).FirstOrDefault());

                            total = total + Decimal.ToInt32(db.Books.Where(m => m.ISBN == bookISBN).Sum(m => m.Price));
                            var bookAmount = count;
                            ViewData[book.Value] = bookAmount;

                            var bookR = db.Books.Where(x => x.ISBN == bookISBN).FirstOrDefault();
                            totalPrice += bookR.Price * book.Count;

                            cartQuantity.Add(bookR, count);
                            bookReturn.Add(bookR);
                        }
                        
                        ViewBag.totalPrice = totalPrice;
                    }
                    catch (FormatException)
                    {

                    }
                }
            }
            Session["Cart"] = cartQuantity;
            return View(bookReturn);
        }

        public ActionResult OrderStatus()
        {
            ViewBag.Message = "THIS IS THE ORDER STATUS PAGE";

            if (Session["User_id"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }
    }
}