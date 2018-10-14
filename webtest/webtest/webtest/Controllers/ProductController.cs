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
        public ActionResult Results(string search, string Category, string Rating, string MinPrice, string MaxPrice, int? page)
        {   
            // De producten zullen op de jusite manier worden laten zien en de PagedList werkt ook gewoon nog.
            // Het is belangrijk dat de View Een .ToList().TopPagedList() returned zodat er door de producten geloopt kan worden en zodat de PagedList goed werkt.
            List<string> ratings = new List<string>() { "1", "2", "3", "4", "5" };

            

            if (search == "")
            { 
                return View(db.Books.Where(m => m.Name.Contains("*#*#*@")).ToList().ToPagedList(page ?? 1, 3));
            }


            if (MinPrice != null)
            {
                if (MinPrice != "")
                {
                    if (MaxPrice != null)
                    {
                        if (MaxPrice != "")
                        {
                            if (ratings.Contains(Rating))
                            {
                                try
                                {
                                    decimal MinPriceD = Convert.ToDecimal(MinPrice);
                                    decimal MaxPriceD = Convert.ToDecimal(MaxPrice);
                                    int rating = Convert.ToInt32(Rating);
                                    if(Category != null)
                                    {
                                        var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                        var select2 = db.Books.Where(m => m.Category.ToLower() == Category.ToLower()).ToList();
                                        var select3 = select2.Where(m => m.Price >= MinPriceD == true).ToList();
                                        var results = select3.Where(m => m.Price <= MaxPriceD == true).ToList();
                                        return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));
                                    }
                                    else
                                    {
                                        var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                        var select2 = select.Where(m => m.Price >= MinPriceD == true).ToList();
                                        var results = select2.Where(m => m.Price <= MaxPriceD == true).ToList();
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
                                    decimal MinPriceD = Convert.ToDecimal(MinPrice);
                                    decimal MaxPriceD = Convert.ToDecimal(MaxPrice);
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var results = select.Where(m => m.Price >= MinPriceD == true).ToList();
                                    return View(results.Where(m => m.Price <= MaxPriceD == true).ToList().ToPagedList(page ?? 1, 3));
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
                                try
                                {
                                    decimal MinPriceD = Convert.ToDecimal(MinPrice);
                                    int rating = Convert.ToInt32(Rating);
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var results = select.Where(m => m.Price >= MinPriceD == true).ToList();
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));
                                }
                                catch (FormatException)
                                {

                                }
                            }
                            else
                            {
                                try
                                {
                                    decimal MinPriceD = Convert.ToDecimal(MinPrice);
                                    var results = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    return View(results.Where(m => m.Price >= MinPriceD == true).ToList().ToPagedList(page ?? 1, 3));
                                }
                                catch (FormatException)
                                {

                                }
                            }
                        }

                    }
                    else
                    {
                        try
                        {
                            decimal MinPriceD = Convert.ToDecimal(MinPrice);
                            var results = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                            return View(results.Where(m => m.Price >= MinPriceD == true).ToList().ToPagedList(page ?? 1, 3));
                        }
                        catch (FormatException)
                        {

                        }
                    }
                }





                else
                {
                    if (MaxPrice != null)
                    {
                        if (MaxPrice != "")
                        {
                            if (!ratings.Contains(Rating))
                            {
                                try
                                {
                                    decimal MaxPriceD = Convert.ToDecimal(MaxPrice);
                                    var results = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    return View(results.Where(m => m.Price <= MaxPriceD == true).ToList().ToPagedList(page ?? 1, 3));
                                }
                                catch (FormatException)
                                {

                                }
                            }
                            else
                            {
                                try
                                {
                                    decimal MaxPriceD = Convert.ToDecimal(MaxPrice);
                                    int rating = Convert.ToInt32(Rating);
                                    var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    var results = select.Where(m => m.Price <= MaxPriceD == true).ToList();
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
                                try
                                {
                                    int rating = Convert.ToInt32(Rating);
                                    var results = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                    return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));
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
                            try
                            {
                                int rating = Convert.ToInt32(Rating);
                                var results = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));
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
                if (MaxPrice != null)
                {
                    if (MaxPrice != "")
                    {
                        if (!ratings.Contains(Rating))
                        {
                            try
                            {
                                decimal MaxPriceD = Convert.ToDecimal(MaxPrice);
                                var results = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                return View(results.Where(m => m.Price <= MaxPriceD == true).ToList().ToPagedList(page ?? 1, 3));
                            }
                            catch (FormatException)
                            {

                            }
                        }
                        else
                        {
                            try
                            {
                                decimal MaxPriceD = Convert.ToDecimal(MaxPrice);
                                int rating = Convert.ToInt32(Rating);
                                var select = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                var results = select.Where(m => m.Price <= MaxPriceD == true).ToList();
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
                            try
                            {
                                int rating = Convert.ToInt32(Rating);
                                var results = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                                return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));
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
                        try
                        {
                            int rating = Convert.ToInt32(Rating);
                            var results = db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList();
                            return View(results.Where(m => m.Rating == rating).ToList().ToPagedList(page ?? 1, 3));
                        }
                        catch (FormatException)
                        {

                        }
                    }

                }
            }


            // NO FILTERS
            return View(db.Books.Where(m => m.Name.Contains(search) || m.Author.Contains(search)).ToList().ToPagedList(page ?? 1, 3));
        }

        [NonAction]
        public decimal ToDecimal(string val)
        {
            return Convert.ToDecimal(val);
        }


    }
}