using System;
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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WishList()
        {
            ViewBag.Message = "THIS IS THE WISHLIST PAGE";
            return View();

        }

        public ActionResult ShoppingCart(string isbn, double? del, bool delete = false, bool plus = false, bool min = false)
        {
            decimal totalPrice = 0;
            string deleteISBN = del.ToString();
            List<Book> bookList = new List<Book>();
            List<Book> bookReturn = new List<Book>();
            Dictionary<Book, int> cartQuantity = new Dictionary<Book, int>();

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
                    double isbnD = Convert.ToDouble(isbn);
                    var book = db.Carts.Where(x => x.ISBN == isbnD).FirstOrDefault();
                    book.Quantity += 1;
                    db.SaveChanges(); 
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
                        isbns.Add(isbn);
                        var newcart = String.Join(",", isbns);
                        Session["shoppingCart"] = newcart;

                        plus = false;
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