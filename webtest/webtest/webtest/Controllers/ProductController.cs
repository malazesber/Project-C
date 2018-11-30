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
            Dictionary<Book, int> cartQuantity = new Dictionary<Book, int>();

            if (favoISBN != "" && favoISBN != null)
            {
                if (Session["User_id"] == null)
                {
                    TempData["favo"] = "<script>alert('You need to login first.');</script>";
                }
                else
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
                            var favoObj = new Favorite() { User_id = User_id, ISBN = Convert.ToDouble(favo)};
                            db.Favorites.Add(favoObj);
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

                    int User_id = Convert.ToInt32(Session["User_id"]);

                    bool has = list.Any(cus => cus.ISBN == cart && cus.User_id == User_id);

                    if (has && plus != true)
                    {
                        using (DatabaseEntities1 db = new DatabaseEntities1())
                        {
                            db.Carts.Remove(db.Carts.Single(cus => cus.ISBN == cart && cus.User_id == User_id));
                            db.SaveChanges();
                        }

                    }
                    else if (plus != true)
                    {
                        using (DatabaseEntities1 db = new DatabaseEntities1())
                        {
                            var cartObj = new Cart() { User_id = User_id, ISBN = Convert.ToDouble(cart), Quantity = 1 };
                            db.Carts.Add(cartObj);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    // UNREGISTERED USER
                    if (Session["shoppingCart"] == null || Session["shoppingCart"] == "")
                    {
                        Session["shoppingCart"] = cartISBN;
                        plus = false;
                    }
                    else if (plus == false)
                    {
                        List<string> isbns = Session["shoppingCart"].ToString().Split(',').ToList();
                        //Check of die al in je cart zit.
                        if (isbns.Contains(cartISBN))
                        {

                            isbns.Remove(cartISBN);
                            var newcart = String.Join(",", isbns);
                            Session["shoppingCart"] = newcart;

                        }
                        else
                        {
                            Session["shoppingCart"] = Session["shoppingCart"] + "," + cartISBN;
                        }
                    }
                }
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

                    var _isbn = isbn.ToString();


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
                else if (type == "favorite")
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

            List<Book> OrderCheck(string GivenOrder, List<Book> results)
            {
                if (orders.Contains(GivenOrder))
                {
                    if (GivenOrder == "Price: Descending")
                    {
                        results = results.OrderByDescending(x => x.Price).ToList();
                        return results;
                    }
                    else if (GivenOrder == "Price: Ascending")
                    {
                        results = results.OrderBy(x => x.Price).ToList();
                        return results;
                    }
                    else if (GivenOrder == "Title: A - Z")
                    {
                        results = results.OrderBy(x => x.Name).ToList();
                        return results;
                    }
                    else if (GivenOrder == "Title: Z - A")
                    {
                        results = results.OrderByDescending(x => x.Name).ToList();
                        return results;
                    }
                    else if (GivenOrder == "Author: A - Z")
                    {
                        results = results.OrderBy(x => x.Author).ToList();
                        return results;
                    }
                    else if (GivenOrder == "Author: Z - A")
                    {
                        results = results.OrderByDescending(x => x.Author).ToList();
                        return results;
                    }
                    else
                    {
                        Console.WriteLine("Could not find the OrderBy value");
                        return results;
                    }
                }
                else return results;
            }


            // ALS SEARCH LEEG IS
            if (search == "" || search == null)
            {
                if (categories.Contains(Category))
                {
                    if (!((MinPrice != null && MinPrice != "") || (MaxPrice != null && MaxPrice != "") || ratings.Contains(Rating) || orders.Contains(Order)))
                    {

                        return View(db.Books.Where(m => m.Category.Contains(Category)).ToList().ToPagedList(page ?? 1, Pagination));
                    }

                }
                else
                {
                    return View(db.Books.Where(m => m.Name.Contains("####@")).ToList().ToPagedList(page ?? 1, Pagination));
                }
            }

            //FILTERS
            if (MinPrice != null && MinPrice != "")
            {
                if (MaxPrice != null && MaxPrice != "")
                {
                    if (ratings.Contains(Rating))
                    {
                        if (categories.Contains(Category))
                        {
                            //MIN PRIJS - MAX PRIJS - RATING - CATEGORY
                            try
                            {
                                decimal MinPriceD = Convert.ToDecimal(MinPrice);
                                decimal MaxPriceD = Convert.ToDecimal(MaxPrice);
                                int rating = Convert.ToInt32(Rating);

                                // ALS SEARCH LEEG IS
                                if (search == "" || search == null)
                                {
                                    var select = db.Books.Where(m => m.Category.Contains(Category)).ToList();
                                    var select2 = select.Where(m => m.Price >= MinPriceD == true).ToList();
                                    var results = select2.Where(m => m.Price <= MaxPriceD == true).ToList();
                                    results = OrderCheck(Order, results);
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, Pagination));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var select2 = select.Where(m => m.Category.Contains(Category)).ToList();
                                    var select3 = select2.Where(m => m.Price >= MinPriceD == true).ToList();
                                    var results = select3.Where(m => m.Price <= MaxPriceD == true).ToList();
                                    results = OrderCheck(Order, results);
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, Pagination));

                                }


                            }
                            catch (FormatException)
                            {

                            }
                        }
                        else
                        {
                            //MIN PRIJS - MAX PRIJS - RATING
                            try
                            {
                                decimal MinPriceD = Convert.ToDecimal(MinPrice);
                                decimal MaxPriceD = Convert.ToDecimal(MaxPrice);
                                int rating = Convert.ToInt32(Rating);

                                var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                var select2 = select.Where(m => m.Price >= MinPriceD == true).ToList();
                                var results = select2.Where(m => m.Price <= MaxPriceD == true).ToList();
                                results = OrderCheck(Order, results);
                                return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, Pagination));

                            }
                            catch (FormatException)
                            {

                            }
                        }

                    }
                    else
                    {
                        if (categories.Contains(Category))
                        {
                            //MIN PRIJS - MAX PRIJS - CATEGORY
                            try
                            {
                                decimal MinPriceD = Convert.ToDecimal(MinPrice);
                                decimal MaxPriceD = Convert.ToDecimal(MaxPrice);

                                // ALS SEARCH LEEG IS
                                if (search == "" || search == null)
                                {
                                    var select = db.Books.Where(m => m.Category.Contains(Category)).ToList();
                                    var results = select.Where(m => m.Price >= MinPriceD == true).ToList();
                                    results = OrderCheck(Order, results);
                                    return View(results.Where(m => m.Price <= MaxPriceD == true).ToList().ToPagedList(page ?? 1, Pagination));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var select2 = select.Where(m => m.Category.Contains(Category)).ToList();
                                    var results = select2.Where(m => m.Price >= MinPriceD == true).ToList();
                                    results = OrderCheck(Order, results);
                                    return View(results.Where(m => m.Price <= MaxPriceD == true).ToList().ToPagedList(page ?? 1, Pagination));

                                }


                            }
                            catch (FormatException)
                            {

                            }
                        }
                        else
                        {
                            //MIN PRIJS - MAX PRIJS
                            try
                            {

                                decimal MinPriceD = Convert.ToDecimal(MinPrice);
                                decimal MaxPriceD = Convert.ToDecimal(MaxPrice);
                                var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                var results = select.Where(m => m.Price >= MinPriceD == true).ToList();
                                results = OrderCheck(Order, results);
                                return View(results.Where(m => m.Price <= MaxPriceD == true).ToList().ToPagedList(page ?? 1, Pagination));
                            }
                            catch (FormatException)
                            {

                            }
                        }

                    }

                }






                else
                {

                    if (ratings.Contains(Rating))
                    {
                        if (categories.Contains(Category))
                        {
                            // MIN PRIJS - RATING - CATEGORY
                            try
                            {
                                decimal MinPriceD = Convert.ToDecimal(MinPrice);
                                int rating = Convert.ToInt32(Rating);

                                // ALS SEARCH LEEG IS
                                if (search == "" || search == null)
                                {
                                    var select = db.Books.Where(m => m.Category.Contains(Category)).ToList();
                                    var results = select.Where(m => m.Price >= MinPriceD == true).ToList();
                                    results = OrderCheck(Order, results);
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, Pagination));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var select2 = select.Where(m => m.Category.Contains(Category)).ToList();
                                    var results = select2.Where(m => m.Price >= MinPriceD == true).ToList();
                                    results = OrderCheck(Order, results);
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, Pagination));

                                }


                            }
                            catch (FormatException)
                            {

                            }
                        }
                        else
                        {
                            // MIN PRIJS - RATING
                            try
                            {
                                decimal MinPriceD = Convert.ToDecimal(MinPrice);
                                int rating = Convert.ToInt32(Rating);
                                var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                var results = select.Where(m => m.Price >= MinPriceD == true).ToList();
                                results = OrderCheck(Order, results);
                                return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, Pagination));
                            }
                            catch (FormatException)
                            {

                            }
                        }

                    }
                    else
                    {
                        if (categories.Contains(Category))
                        {
                            // MIN PRIJS - CATEGORY
                            try
                            {
                                decimal MinPriceD = Convert.ToDecimal(MinPrice);

                                // ALS SEARCH LEEG IS
                                if (search == "" || search == null)
                                {
                                    var results = db.Books.Where(m => m.Category.Contains(Category)).ToList();
                                    results = OrderCheck(Order, results);
                                    return View(results.Where(m => m.Price >= MinPriceD == true).ToList().ToPagedList(page ?? 1, Pagination));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var results = select.Where(m => m.Category.Contains(Category)).ToList();
                                    results = OrderCheck(Order, results);
                                    return View(results.Where(m => m.Price >= MinPriceD == true).ToList().ToPagedList(page ?? 1, Pagination));

                                }


                            }
                            catch (FormatException)
                            {

                            }
                        }
                        else
                        {
                            // MIN PRIJS
                            try
                            {
                                decimal MinPriceD = Convert.ToDecimal(MinPrice);
                                var results = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                results = OrderCheck(Order, results);
                                return View(results.Where(m => m.Price >= MinPriceD == true).ToList().ToPagedList(page ?? 1, Pagination));
                            }
                            catch (FormatException)
                            {

                            }
                        }
                    }


                }
            }
            else
            {
                if (MaxPrice != null && MaxPrice != "")
                {

                    if (!ratings.Contains(Rating))
                    {
                        if (categories.Contains(Category))
                        {
                            // MAX PRIJS - CATEGORY
                            try
                            {
                                decimal MaxPriceD = Convert.ToDecimal(MaxPrice);

                                // ALS SEARCH LEEG IS
                                if (search == "" || search == null)
                                {
                                    var results = db.Books.Where(m => m.Category.Contains(Category)).ToList();
                                    if (orders.Contains(Order))
                                    {
                                        results = OrderCheck(Order, results);
                                    }
                                    return View(results.Where(m => m.Price <= MaxPriceD == true).ToList().ToPagedList(page ?? 1, Pagination));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var results = select.Where(m => m.Category.Contains(Category)).ToList();
                                    results = OrderCheck(Order, results);
                                    return View(results.Where(m => m.Price <= MaxPriceD == true).ToList().ToPagedList(page ?? 1, Pagination));

                                }


                            }
                            catch (FormatException)
                            {

                            }
                        }
                        else
                        {
                            // MAX RPIJS
                            try
                            {
                                decimal MaxPriceD = Convert.ToDecimal(MaxPrice);
                                var results = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                results = OrderCheck(Order, results);
                                return View(results.Where(m => m.Price <= MaxPriceD == true).ToList().ToPagedList(page ?? 1, Pagination));
                            }
                            catch (FormatException)
                            {

                            }
                        }

                    }
                    else
                    {
                        if (categories.Contains(Category))
                        {
                            //  MAX PRIJS - RATING - CATEGORY
                            try
                            {
                                decimal MaxPriceD = Convert.ToDecimal(MaxPrice);
                                int rating = Convert.ToInt32(Rating);

                                // ALS SEARCH LEEG IS
                                if (search == "" || search == null)
                                {
                                    var select = db.Books.Where(m => m.Category.Contains(Category)).ToList();
                                    var results = select.Where(m => m.Price <= MaxPriceD == true).ToList();
                                    results = OrderCheck(Order, results);
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, Pagination));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var select2 = select.Where(m => m.Category.Contains(Category)).ToList();
                                    var results = select2.Where(m => m.Price <= MaxPriceD == true).ToList();
                                    results = OrderCheck(Order, results);
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, Pagination));

                                }


                            }
                            catch (FormatException)
                            {

                            }
                        }
                        // MAX PRIJS - RATING
                        try
                        {
                            decimal MaxPriceD = Convert.ToDecimal(MaxPrice);
                            int rating = Convert.ToInt32(Rating);
                            var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                            var results = select.Where(m => m.Price <= MaxPriceD == true).ToList();
                            results = OrderCheck(Order, results);
                            return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, Pagination));
                        }
                        catch (FormatException)
                        {

                        }
                    }

                }
                else
                {
                    if (ratings.Contains(Rating))
                    {
                        if (categories.Contains(Category))
                        {
                            //  RATING - CATEGORY
                            try
                            {
                                int rating = Convert.ToInt32(Rating);

                                // ALS SEARCH LEEG IS
                                if (search == "" || search == null)
                                {
                                    var results = db.Books.Where(m => m.Category.Contains(Category)).ToList();
                                    results = OrderCheck(Order, results);
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, Pagination));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var results = select.Where(m => m.Category.Contains(Category)).ToList();
                                    results = OrderCheck(Order, results);
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, Pagination));
                                }


                            }
                            catch (FormatException)
                            {

                            }
                        }
                        else
                        {
                            try
                            {
                                // RATING
                                int rating = Convert.ToInt32(Rating);
                                var results = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                results = OrderCheck(Order, results);
                                return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, Pagination));
                            }
                            catch (FormatException)
                            {

                            }
                        }

                    }

                }
            }



            // NO FILTERS
            if (categories.Contains(Category))
            {


                // ALS SEARCH LEEG IS
                if (search == "" || search == null)
                {
                    var results = db.Books.Where(m => m.Category.Contains(Category)).ToList();
                    results = OrderCheck(Order, results);
                    return View(results.ToPagedList(page ?? 1, Pagination));
                }
                //ALS SEARCH ACTIEF IS
                else
                {
                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                    var results = select.Where(m => m.Category.Contains(Category)).ToList();
                    results = OrderCheck(Order, results);
                    return View(results.ToPagedList(page ?? 1, Pagination));
                }

            }
            else
            {
                var results = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                results = OrderCheck(Order, results);
                return View(results.ToPagedList(page ?? 1, Pagination));
            }

        }


    }
}
