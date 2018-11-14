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
        // GET: Product
        public ActionResult Index(string Title)
        {
            string bookTitle = Title;

            //Kiest de titel van het boek dat overeenkomst met die titel die Index ontvangt.
            //Id of ISBN zou beter zijn, maar wist niet welke we nou als PK hadden. Is makkelijk aan te passen later.
            return View(db.Books.Where(m => m.Name == bookTitle).FirstOrDefault());
        }

        public ActionResult Results(string search, string Category, string Rating, string MinPrice, string MaxPrice, int? page, string Order, string isbn, int Pagination = 5)
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
                if (Session["User_id"] == null)
                {
                    TempData["msg"] = "<script>alert('You need to login first.');</script>";
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
                        TempData["msg"] = "<script>alert('Removed from favorites');</script>";
                    }
                    else
                    {
                        // ISBN TOEVOEGEN AAN FAVORIETEN
                        TempData["msg"] = "<script>alert('Added to Favorites');</script>";
                        using (DatabaseEntities1 db = new DatabaseEntities1())
                        {
                            var favo = new Favorite() { User_id = User_id, ISBN = Convert.ToDouble(isbn) };
                            db.Favorites.Add(favo);
                            db.SaveChanges();
                        }
                    }
                }


            }

            List<Book> OrderCheck(string GivenOrder, List<Book> results)
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
                                    if (orders.Contains(Order))
                                    {
                                        results = OrderCheck(Order, results);
                                    }
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, Pagination));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var select2 = select.Where(m => m.Category.Contains(Category)).ToList();
                                    var select3 = select2.Where(m => m.Price >= MinPriceD == true).ToList();
                                    var results = select3.Where(m => m.Price <= MaxPriceD == true).ToList();
                                    if (orders.Contains(Order))
                                    {
                                        results = OrderCheck(Order, results);
                                    }
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
                                if (orders.Contains(Order))
                                {
                                    results = OrderCheck(Order, results);
                                }
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
                                    var select2 = select.Where(m => m.Category.Contains(Category)).ToList();
                                    var results = select2.Where(m => m.Price >= MinPriceD == true).ToList();
                                    if (orders.Contains(Order))
                                    {
                                        results = OrderCheck(Order, results);
                                    }
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
                                if (orders.Contains(Order))
                                {
                                    results = OrderCheck(Order, results);
                                }
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
                                    if (orders.Contains(Order))
                                    {
                                        results = OrderCheck(Order, results);
                                    }
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, Pagination));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var select2 = select.Where(m => m.Category.Contains(Category)).ToList();
                                    var results = select2.Where(m => m.Price >= MinPriceD == true).ToList();
                                    if (orders.Contains(Order))
                                    {
                                        results = OrderCheck(Order, results);
                                    }
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
                                if (orders.Contains(Order))
                                {
                                    results = OrderCheck(Order, results);
                                }
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
                                    if (orders.Contains(Order))
                                    {
                                        results = OrderCheck(Order, results);
                                    }
                                    return View(results.Where(m => m.Price >= MinPriceD == true).ToList().ToPagedList(page ?? 1, Pagination));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var results = select.Where(m => m.Category.Contains(Category)).ToList();
                                    if (orders.Contains(Order))
                                    {
                                        results = OrderCheck(Order, results);
                                    }
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
                                if (orders.Contains(Order))
                                {
                                    results = OrderCheck(Order, results);
                                }
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
                                    if (orders.Contains(Order))
                                    {
                                        results = OrderCheck(Order, results);
                                    }
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
                                if (orders.Contains(Order))
                                {
                                    results = OrderCheck(Order, results);
                                }
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
                                    if (orders.Contains(Order))
                                    {
                                        results = OrderCheck(Order, results);
                                    }
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, Pagination));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var select2 = select.Where(m => m.Category.Contains(Category)).ToList();
                                    var results = select2.Where(m => m.Price <= MaxPriceD == true).ToList();
                                    if (orders.Contains(Order))
                                    {
                                        results = OrderCheck(Order, results);
                                    }
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
                            if (orders.Contains(Order))
                            {
                                results = OrderCheck(Order, results);
                            }
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
                                    if (orders.Contains(Order))
                                    {
                                        results = OrderCheck(Order, results);
                                    }
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, Pagination));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var results = select.Where(m => m.Category.Contains(Category)).ToList();
                                    if (orders.Contains(Order))
                                    {
                                        results = OrderCheck(Order, results);
                                    }
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
                                if (orders.Contains(Order))
                                {
                                    results = OrderCheck(Order, results);
                                }
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
                    if (orders.Contains(Order))
                    {
                        results = OrderCheck(Order, results);
                    }
                    return View(results.ToPagedList(page ?? 1, Pagination));
                }
                //ALS SEARCH ACTIEF IS
                else
                {
                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                    var results = select.Where(m => m.Category.Contains(Category)).ToList();
                    if (orders.Contains(Order))
                    {
                        results = OrderCheck(Order, results);
                    }
                    return View(results.ToPagedList(page ?? 1, Pagination));
                }

            }
            else
            {
                var results = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                if (orders.Contains(Order))
                {
                    results = OrderCheck(Order, results);
                }
                return View(results.ToPagedList(page ?? 1, Pagination));
            }

        }


    }
}
