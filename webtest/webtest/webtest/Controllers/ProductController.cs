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
        public ActionResult Index(double? favo, double? cart, double? del, double isbn = 0, string date = "", bool addreview = false, int readMore = 0, bool delete = false, bool plus = false, bool min = false, decimal rating = 0, string review = "", string name = "", string surname = "", bool deleteReview = false, int deleteId = 0)
        {
            string favoISBN = favo.ToString();
            string cartISBN = cart.ToString();
            var _isbn = isbn.ToString();
            Dictionary<Book, int> cartQuantity = new Dictionary<Book, int>();
            int User_id = 0;

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
                User_id = Convert.ToInt32(Session["User_id"]);

                // Session voor de input fields
                Review(_isbn);


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

            // Adds the review to the review table in the database
            if (addreview)
            {
                DateTime _date = Convert.ToDateTime(date);
                int _user_id = Convert.ToInt32(Session["User_id"]);
          
                var maxId = db.Reviews.Max(m => m.Id);

                var newReview = new Review()
                {
                    Id = maxId + 1,
                    Name = name,
                    Surname = surname,
                    Rating = rating,
                    Date = _date,
                    Review1 = review,
                    ISBN = isbn,
                    User_id = _user_id

                };

                db.Reviews.Add(newReview);
                db.SaveChanges();

                // Hides the input fields in the view
                Session["ReviewInput"] = null;

            }

            // Admin can delete reviews on the product page
            if (deleteReview)
            {
                using (DatabaseEntities1 db = new DatabaseEntities1())
                {
                    db.Reviews.Remove(db.Reviews.Single(x => x.Id == deleteId));
                    db.SaveChanges();
                }
            }

            //Read more button
            using (DatabaseEntities1 db = new DatabaseEntities1())
            {
                var _summary = (from book in db.Books
                                where book.ISBN == isbn
                                select book.Summary).FirstOrDefault();

                if (_summary != null)
                {
                    var summaryLength = _summary.Count();

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

            Tuple<Book, Review, User> productInfo;
            var returnBook = db.Books.Where(m => m.ISBN == isbn).FirstOrDefault();
            var returnReview = db.Reviews.Where(m => m.ISBN == isbn).FirstOrDefault();
            var returnUser = db.Users.Where(m => m.User_id == User_id).FirstOrDefault();

            productInfo = new Tuple<Book, Review, User>(returnBook, returnReview, returnUser);

            return View(productInfo);
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
            if (search != null)
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

            //if (ratings.Contains(Rating))
            //{
            //    int rating = Convert.ToInt32(Rating);
            //    select = select.Where(m => m.Rating == rating).ToList();
            //}

            if (ratings.Contains(Rating))
            {
                int rating = Convert.ToInt32(Rating);

                select = select.Where(m =>
                {
                    // Checks if there is a rating based on reviews by users
                    try
                    {
                        Decimal average = (from r in db.Reviews
                                       where r.ISBN == m.ISBN
                                       select r.Rating).Average();

                        bool ratingFromReview = average >= rating &&
                                                average < rating + 1;

                        return ratingFromReview;

                    }

                    // Gets the old rating in the book table
                    catch
                    {
                        bool ratingFromBook = m.Rating >= rating && m.Rating < rating + 1;
                        return ratingFromBook;
                    }
                }
            ).ToList();

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

        public void Review(string isbn)
        {
            var list = db.Reviews.Select(s => s);
            double isbnD = Convert.ToDouble(isbn);
            int User_id = Convert.ToInt32(Session["User_id"]);

            // Selects a list with all order numbers linked to the user account
            var listOrderNumbers = (from o in db.Orders
                        where o.User_id == User_id
                        select o.Order_Number).ToList();

            // Selects the products the user bought
            var listBoughtProducts = (from t in listOrderNumbers
                         from od in db.OrderDetails
                         where t == od.Order_Number
                         select od.Products).ToList();

            // New list for the ISBNS the user bought
            List<double> ISBNboughtProducts = new List<double>();

            // Gets the ISBNS
            foreach (var x in listBoughtProducts)
            {
                var products = x.Split('|');

                foreach (var item in products)
                {
                    string[] books = item.Split('-');
                    double _isbn = Convert.ToDouble(books[0]);

                    ISBNboughtProducts.Add(_isbn);
                }
            }

            // Checks if the user bought the book
            bool boughtBook = ISBNboughtProducts.Contains(isbnD);

            // Checks if the user already wrote a review
            bool wroteReview = list.ToList().Any(r => r.ISBN == isbnD && r.User_id == User_id);

            // Session["ReviewInput"] includes the input fields in the view
            if (boughtBook)
            {
                Session["ReviewInput"] = true;
            }
            if (wroteReview)
            {
                Session["ReviewInput"] = null;
            }
            else
            {
                Session["ReviewInput"] = null;
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