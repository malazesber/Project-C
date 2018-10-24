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
        //COnnectie met database
        DatabaseEntities1 db = new DatabaseEntities1();
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Results(string search, string Category, string Rating, string MinPrice, string MaxPrice, int? page, string Order)
        {
            // De producten zullen op de jusite manier worden laten zien en de PagedList werkt ook gewoon nog.
            // Het is belangrijk dat de View Een .ToList().TopPagedList() returned zodat er door de producten geloopt kan worden en zodat de PagedList goed werkt.
            List<string> ratings = new List<string>() { "1", "2", "3", "4", "5" };
            List<string> categories = new List<string>() { "Parenting", "Food & Drink", "History & Politics", "Home & Garden", "Mind Body Spirit",
            "Science & Nature", "Sports", "Style & Beauty", "Fiction", "Education", "Diet & Fitness", "Business", "Biography", "Art & Photography"};
            List<string> orders = new List<string>() { "Price ascending", "Price descending" };

            // ALS SEARCH LEEG IS
            if (search == "" || search == null)
            {
                if (categories.Contains(Category))
                {
                    if (!((MinPrice != null && MinPrice != "") || (MaxPrice != null && MaxPrice != "") || ratings.Contains(Rating) || orders.Contains(Order)))
                    {

                        return View(db.Books.Where(m => m.Category.Contains(Category)).ToList().ToPagedList(page ?? 1, 3));
                    }
                        
                }
                else
                {
                    return View(db.Books.Where(m => m.Name.Contains("####@")).ToList().ToPagedList(page ?? 1, 3));
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
                                        if (Order == "Price descending")
                                        {
                                            results = results.OrderByDescending(x => x.Price).ToList();
                                        }
                                        else
                                        {
                                            results = results.OrderBy(x => x.Price).ToList();
                                        }
                                    }
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));
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
                                        if (Order == "Price descending")
                                        {
                                            results = results.OrderByDescending(x => x.Price).ToList();
                                        }
                                        else
                                        {
                                            results = results.OrderBy(x => x.Price).ToList();
                                        }
                                    }
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));
                                    
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
                                    if (Order == "Price descending")
                                    {
                                        results = results.OrderByDescending(x => x.Price).ToList();
                                    }
                                    else
                                    {
                                        results = results.OrderBy(x => x.Price).ToList();
                                    }
                                }
                                return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));

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
                                        if (Order == "Price descending")
                                        {
                                            results = results.OrderByDescending(x => x.Price).ToList();
                                        }
                                        else
                                        {
                                            results = results.OrderBy(x => x.Price).ToList();
                                        }
                                    }
                                    return View(results.Where(m => m.Price <= MaxPriceD == true).ToList().ToPagedList(page ?? 1, 3));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var select2 = select.Where(m => m.Category.Contains(Category)).ToList();
                                    var results = select2.Where(m => m.Price >= MinPriceD == true).ToList();
                                    if (orders.Contains(Order))
                                    {
                                        if (Order == "Price descending")
                                        {
                                            results = results.OrderByDescending(x => x.Price).ToList();
                                        }
                                        else
                                        {
                                            results = results.OrderBy(x => x.Price).ToList();
                                        }
                                    }
                                    return View(results.Where(m => m.Price <= MaxPriceD == true).ToList().ToPagedList(page ?? 1, 3));

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
                                    if (Order == "Price descending")
                                    {
                                        results = results.OrderByDescending(x => x.Price).ToList();
                                    }
                                    else
                                    {
                                        results = results.OrderBy(x => x.Price).ToList();
                                    }
                                }
                                return View(results.Where(m => m.Price <= MaxPriceD == true).ToList().ToPagedList(page ?? 1, 3));
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
                                        if (Order == "Price descending")
                                        {
                                            results = results.OrderByDescending(x => x.Price).ToList();
                                        }
                                        else
                                        {
                                            results = results.OrderBy(x => x.Price).ToList();
                                        }
                                    }
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var select2 = select.Where(m => m.Category.Contains(Category)).ToList();
                                    var results = select2.Where(m => m.Price >= MinPriceD == true).ToList();
                                    if (orders.Contains(Order))
                                    {
                                        if (Order == "Price descending")
                                        {
                                            results = results.OrderByDescending(x => x.Price).ToList();
                                        }
                                        else
                                        {
                                            results = results.OrderBy(x => x.Price).ToList();
                                        }
                                    }
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));

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
                                    if (Order == "Price descending")
                                    {
                                        results = results.OrderByDescending(x => x.Price).ToList();
                                    }
                                    else
                                    {
                                        results = results.OrderBy(x => x.Price).ToList();
                                    }
                                }
                                return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));
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
                                        if (Order == "Price descending")
                                        {
                                            results = results.OrderByDescending(x => x.Price).ToList();
                                        }
                                        else
                                        {
                                            results = results.OrderBy(x => x.Price).ToList();
                                        }
                                    }
                                    return View(results.Where(m => m.Price >= MinPriceD == true).ToList().ToPagedList(page ?? 1, 3));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                var results = select.Where(m => m.Category.Contains(Category)).ToList();
                                if (orders.Contains(Order))
                                {
                                    if (Order == "Price descending")
                                    {
                                        results = results.OrderByDescending(x => x.Price).ToList();
                                    }
                                    else
                                    {
                                        results = results.OrderBy(x => x.Price).ToList();
                                    }
                                }
                                return View(results.Where(m => m.Price >= MinPriceD == true).ToList().ToPagedList(page ?? 1, 3));

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
                                    if (Order == "Price descending")
                                    {
                                        results = results.OrderByDescending(x => x.Price).ToList();
                                    }
                                    else
                                    {
                                        results = results.OrderBy(x => x.Price).ToList();
                                    }
                                }
                                return View(results.Where(m => m.Price >= MinPriceD == true).ToList().ToPagedList(page ?? 1, 3));
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
                                        if (Order == "Price descending")
                                        {
                                            results = results.OrderByDescending(x => x.Price).ToList();
                                        }
                                        else
                                        {
                                            results = results.OrderBy(x => x.Price).ToList();
                                        }
                                    }
                                    return View(results.Where(m => m.Price <= MaxPriceD == true).ToList().ToPagedList(page ?? 1, 3));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var results = select.Where(m => m.Category.Contains(Category)).ToList();
                                    if (orders.Contains(Order))
                                    {
                                        if (Order == "Price descending")
                                        {
                                            results = results.OrderByDescending(x => x.Price).ToList();
                                        }
                                        else
                                        {
                                            results = results.OrderBy(x => x.Price).ToList();
                                        }
                                    }
                                    return View(results.Where(m => m.Price <= MaxPriceD == true).ToList().ToPagedList(page ?? 1, 3));

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
                                    if (Order == "Price descending")
                                    {
                                        results = results.OrderByDescending(x => x.Price).ToList();
                                    }
                                    else
                                    {
                                        results = results.OrderBy(x => x.Price).ToList();
                                    }
                                }
                                return View(results.Where(m => m.Price <= MaxPriceD == true).ToList().ToPagedList(page ?? 1, 3));
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
                                        if (Order == "Price descending")
                                        {
                                            results = results.OrderByDescending(x => x.Price).ToList();
                                        }
                                        else
                                        {
                                            results = results.OrderBy(x => x.Price).ToList();
                                        }
                                    }
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var select2 = select.Where(m => m.Category.Contains(Category)).ToList();
                                    var results = select2.Where(m => m.Price <= MaxPriceD == true).ToList();
                                    if (orders.Contains(Order))
                                    {
                                        if (Order == "Price descending")
                                        {
                                            results = results.OrderByDescending(x => x.Price).ToList();
                                        }
                                        else
                                        {
                                            results = results.OrderBy(x => x.Price).ToList();
                                        }
                                    }
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));

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
                                if (Order == "Price descending")
                                {
                                    results = results.OrderByDescending(x => x.Price).ToList();
                                }
                                else
                                {
                                    results = results.OrderBy(x => x.Price).ToList();
                                }
                            }
                            return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));
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
                                        if (Order == "Price descending")
                                        {
                                            results = results.OrderByDescending(x => x.Price).ToList();
                                        }
                                        else
                                        {
                                            results = results.OrderBy(x => x.Price).ToList();
                                        }
                                    }
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));
                                }
                                //ALS SEARCH ACTIEF IS
                                else
                                {
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var results = select.Where(m => m.Category.Contains(Category)).ToList();
                                    if (orders.Contains(Order))
                                    {
                                        if (Order == "Price descending")
                                        {
                                            results = results.OrderByDescending(x => x.Price).ToList();
                                        }
                                        else
                                        {
                                            results = results.OrderBy(x => x.Price).ToList();
                                        }
                                    }
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));
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
                                    if (Order == "Price descending")
                                    {
                                        results = results.OrderByDescending(x => x.Price).ToList();
                                    }
                                    else
                                    {
                                        results = results.OrderBy(x => x.Price).ToList();
                                    }
                                }
                                return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));
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
                            if (Order == "Price descending")
                            {
                                results = results.OrderByDescending(x => x.Price).ToList();
                            }
                            else
                            {
                                results = results.OrderBy(x => x.Price).ToList();
                            }
                        }
                    return View(results.ToPagedList(page ?? 1, 3));
                    }
                    //ALS SEARCH ACTIEF IS
                    else
                    {
                        var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                        var results = select.Where(m => m.Category.Contains(Category)).ToList();
                        if (orders.Contains(Order))
                        {
                            if (Order == "Price descending")
                            {
                                results = results.OrderByDescending(x => x.Price).ToList();
                            }
                            else
                            {
                                results = results.OrderBy(x => x.Price).ToList();
                            }
                        }
                        return View(results.ToPagedList(page ?? 1, 3));
                    }

            }
            else
            {
                var results = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                if (orders.Contains(Order))
                {
                    if (Order == "Price descending")
                    {
                        results = results.OrderByDescending(x => x.Price).ToList();
                    }
                    else
                    {
                        results = results.OrderBy(x => x.Price).ToList();
                    }
                }
                return View(results.ToPagedList(page ?? 1, 3));
            }

        }

        [NonAction]
        public List<Book> SetOrder(List<Book> result, string order)
        {
            if(order == "Price descending")
            {
                result.OrderByDescending(x => x.Price);
            }
            else
            {
                result.OrderBy(x => x.Price);
            }

            return result;
        }


    }
}