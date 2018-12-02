using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webtest.Models;
using PagedList;
using PagedList.Mvc;

namespace webtest.Controllers
{
    public class ProductController : Controller
    {
        //Connectie met database
        DatabaseEntities1 db = new DatabaseEntities1();
        List<string> cart = new List<string>();
        // GET: Product
        public ActionResult Index(string Title, double? isbn, double? favo, double? cart, double? del, int readMore = 0, bool delete = false, bool plus = false, bool min = false)
        {
            string favoISBN = favo.ToString();
            string cartISBN = cart.ToString();
            var _isbn = isbn.ToString();
            Dictionary<Book, int> cartQuantity = new Dictionary<Book, int>();

            if (favoISBN != "" && favoISBN != null)
            {
                FavoriteList(_isbn);
            }

            //Cart button toevoegen
            if (cartISBN != "" && cartISBN != null)
            {
                ShoppingCart(_isbn);
            }

            decimal totalPrice = 0;
            string deleteISBN = del.ToString();

            List<Book> bookList = new List<Book>();
            List<Book> bookReturn = new List<Book>();

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
                    if (book == null)
                    {
                        var cartObj = new Cart() { User_id = User_id, ISBN = Convert.ToDouble(cart), Quantity = 1 };
                        db.Carts.Add(cartObj);
                    }
                    else
                    {
                        book.Quantity += 1;
                    }
                    db.SaveChanges();
                }

                if (min)
                {
                    double isbnD = Convert.ToDouble(isbn);
                    var book = db.Carts.Where(x => x.ISBN == isbnD).FirstOrDefault();
                    if (book.Quantity == 1)
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

                    //Deletes product from cart
                    if (isbns.Contains(_isbn) && delete)
                    {
                        isbns.RemoveAll(s => _isbn == s);
                        var newcart = String.Join(",", isbns);
                        Session["shoppingCart"] = newcart;
                        delete = false;
                    }

                    if (plus)
                    {
                        isbns.Add(_isbn);
                        var newcart = String.Join(",", isbns);
                        Session["shoppingCart"] = newcart;

                        plus = false;
                    }

                    if (min)
                    {
                        isbns.Remove(_isbn);
                        var newcart = String.Join(",", isbns);
                        Session["shoppingCart"] = newcart;

                        min = false;
                    }
                    try
                    {

                        foreach (var book in bookListQ)
                        {
                            int count = Convert.ToInt32(book.Count);

                            var bookAmount = count;
                            ViewData[book.Value] = bookAmount;
                        }

                        ViewBag.totalPrice = totalPrice;
                    }
                    catch (FormatException)
                    {

                    }
                }
            }

            //Read more button
            using (DatabaseEntities1 db = new DatabaseEntities1())
            {
                var _summary = (from book in db.Books
                                where book.ISBN == isbn
                                select book.Summary).FirstOrDefault();

                var summaryLength = _summary.Count();


                if (_summary != null)
                {
                    if (readMore == 0)
                    {
                        if (summaryLength > 625)
                        {
                            int pos = _summary.LastIndexOf(" ", 625);

                            _summary = _summary.Substring(0, pos) + "...";

                            ViewBag.summary = _summary;
                            ViewBag.Text = "Read more";
                            ViewBag.Code = 1;
                        }
                        else
                        {
                            ViewBag.summary = _summary;
                            //ViewBag.Text = "Read more";
                            ViewBag.Code = 1;
                        }
                    }
                    else
                    {
                        ViewBag.summary = _summary;
                        ViewBag.Text = "Read less";
                        ViewBag.Code = 0;
                    }
                }
            }

            //Kiest de titel van het boek dat overeenkomst met die titel die Index ontvangt. 
            string bookTitle = Title;
            return View(db.Books.Where(m => m.Name == bookTitle).FirstOrDefault());
        }

        public ActionResult Results(string search, string Category, string Rating, string MinPrice, string MaxPrice, int? page, string Order, string isbn, string type, int Pagination = 5)
        {
            // De producten zullen op de jusite manier worden laten zien en de PagedList werkt ook gewoon nog.
            // Het is belangrijk dat de View Een .ToList().TopPagedList() returned zodat er door de producten geloopt kan worden en zodat de PagedList goed werkt.
            List<string> ratings = new List<string>() { "1", "2", "3", "4", "5" };
            List<string> categories = new List<string>() { "Parenting", "Food & Drink", "History & Politics", "Home & Garden", "Mind Body Spirit",
            "Science & Nature", "Sports", "Style & Beauty", "Fiction", "Education", "Diet & Fitness", "Business", "Biography", "Art & Photography"};
            List<string> orders = new List<string>() { "Price: Ascending", "Price: Descending", "Title: A - Z", "Title: Z - A", "Author: A -Z", "Author: Z - A" };


            // CHECK OF ER EEN ISBN IS MEEGEGEVEN
            if (isbn != "" && isbn != null)
            {
                if (type == "cart")
                {
                    ShoppingCart(isbn);
                }

                else if (type == "favorite")
                {
                    FavoriteList(isbn);
                }
            }

            // Apply filters
            var select = db.Books.ToList();
            if(search != null)
            {
                select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
            }
            if (MinPrice != null && MinPrice != "")
            {
                decimal MinPriceD = Convert.ToDecimal(MinPrice);
                select = select.Where(m => m.Price >= MinPriceD == true).ToList();
            }
            if (MaxPrice != null && MaxPrice != "")
            {
                decimal MaxPriceD = Convert.ToDecimal(MaxPrice);
                select = select.Where(m => m.Price <= MaxPriceD == true).ToList();
            }
            if (categories.Contains(Category))
            {
                select = select.Where(m => m.Category.Contains(Category)).ToList();
            }
            if (ratings.Contains(Rating))
            {
                int rating = Convert.ToInt32(Rating);
                select = select.Where(m => m.Rating == rating).ToList();
            }

            // Apply ordering
            if (orders.Contains(Order))
            {
                switch (Order)
                {
                    case "Price: Descending":
                        select = select.OrderByDescending(x => x.Price).ToList();
                        break;
                    case "Price: Ascending":
                        select = select.OrderBy(x => x.Price).ToList();
                        break;
                    case "Title: A - Z":
                        select = select.OrderBy(x => x.Name).ToList();
                        break;
                    case "Title: Z - A":
                        select = select.OrderByDescending(x => x.Name).ToList();
                        break;
                    case "Author: A - Z":
                        select = select.OrderBy(x => x.Author).ToList();
                        break;
                    case "Author: Z - A":
                        select = select.OrderByDescending(x => x.Author).ToList();
                        break;
                    default:
                        Console.WriteLine("Could not find the OrderBy value");
                        break;
                }
            }

            // Return result
            return View(select.ToPagedList(page ?? 1, Pagination));
        }

        public void ShoppingCart(string isbn)
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
                        var cart = new Cart() { User_id = User_id, ISBN = Convert.ToDouble(isbn), Quantity = 1 };
                        db.Carts.Add(cart);
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
                    if (isbns.Contains(isbn))
                    {
                        isbns.RemoveAll(s => isbn == s);
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

        public void FavoriteList(string isbn)
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
                        var favo = new Favorite() { User_id = User_id, ISBN = Convert.ToDouble(isbn) };
                        db.Favorites.Add(favo);
                        db.SaveChanges();
                    }
                }
            }
        }

    }
}
