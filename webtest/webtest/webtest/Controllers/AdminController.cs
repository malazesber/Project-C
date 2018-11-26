using System;
using System.Collections.Generic;
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
    }
}