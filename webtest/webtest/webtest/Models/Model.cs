//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System;
//using System.Text.RegularExpressions;


//namespace Webshop.Models //TODO: Foreign keys & correcte constraints. Ook datatype check just to be sure
//{
//    public class Book //PK -> ISBN
//    {
//        public string Category { get; set; }

//        //[Key]
//        [Display(Name = "ISBN")]
//        [Required(AllowEmptyStrings = false, ErrorMessage = "Please insert an ISBN number.")]
//        [RegularExpression("([0-9]+){13,15}", ErrorMessage = "The ISBN must consist of numbers only. The minimum amount is 13 numbers, and the maximum amount is 15")]
//        public float ISBN { get; set; }

//        [Display(Name = "Name")]
//        [Required(AllowEmptyStrings = false, ErrorMessage = "Please insert a name for the book.")]
//        [RegularExpression("(^[a-zA-Z0-9 ,.'@ ]+${2,50}", ErrorMessage = "Minimum 2 characters and maximum 50 characters allowed. Please start the name with a capital letter.")]
//        public string Name { get; set; }

//        [Display(Name = "Author")]
//        [Required(AllowEmptyStrings = true)]
//        [RegularExpression("[a-zA-Z\\s]+", ErrorMessage = "The author name can not contain numbers or symbols.")]
//        public string Author { get; set; }

//        [DataType(DataType.Date)]
//        [Display(Name = "Date")]
//        [Required(AllowEmptyStrings = false, ErrorMessage="Please insert a date.")]
//        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
//        public DateTime Date { get; set; } //Is hier een betere datatype voor of iets dergelijks? Misschien een dag,maand,jaar tuple?

//        [DataType(DataType.MultilineText)]
//        [Display(Name = "Summary")]
//        [Required(AllowEmptyStrings = true)]
//        [RegularExpression(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$")]
//        [MaxLength(500, ErrorMessage = "Summary must be up to 500 characters long.")]
//        public string Summary { get; set; }

        
//        public string Image_src { get; set; } //https://stackoverflow.com/questions/42183640/show-image-from-database-in-mvc

//        [Display(Name="Price")]
//        [Required(AllowEmptyStrings =false, ErrorMessage ="Please insert price.")]
//        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage ="Please insert a valid price, using only numbers.")]
//        public decimal Price { get; set; }

//        [Display(Name="Rating")]
//        [Required(AllowEmptyStrings = false, ErrorMessage ="Please insert rating.")]
//        [RegularExpression("^([1-5])$", ErrorMessage ="Please insert a number between 1 and 5.")]
//        public int Rating { get; set; }

//        [Display(Name="Stock")]
//        [Required(AllowEmptyStrings =false, ErrorMessage ="Please insert stock quantity.")]
//        [RegularExpression("^[0-9]*$", ErrorMessage ="Please insert a valid stock quantity.")]
//        [MinLength(1, ErrorMessage = "The minimum amount of valid stock quantity is 1 book.")]
//        public int Stock { get; set; }
//    }



    //public class Orders //PK -> Order_id
    //{
    //    [Key]
    //    public int Order_id { get; set; }
    //    public double Amount { get; set; }
    //    public string Order_status { get; set; }
    //    public int PaymentId { get; set; }//Adds Foreign key to Payment(payment_id) http://www.entityframeworktutorial.net/code-first/configure-one-to-one-relationship-in-code-first.aspx
    //    public virtual Payment Payment { get; set; } //Is bovendien onderdeel van FK
    //    //TODO: Add Foreign key to Shopping_cart(cart_id)

    //}

    //public class Payment //PK -> Payment_id
    //{
    //    [Key]
    //    public int Payment_id { get; set; } //In de sql code staat er dat het varchar is, dan vgm is de juiste c# equivalent: string
    //    public DateTime payment_date { get; set; }
    //    public double Amount { get; set; }
    //    public int OrderId { get; set; }//Adds Foreign key naar Orders(Order_id)
    //    public virtual Orders Orders { get; set; } //is bovendien ook onderdeel van FK
    //}

    //public class Wishlist //PK -> List_id
    //{
    //    [Key]
    //    public string List_id { get; set; }
    //    public string user_id { get; set; }
    //}

    //public class Creditcard_details //PK -> CC_number
    //{
    //    [Key]
    //    public string CC_number { get; set; }
    //    public string Owner_name { get; set; }
    //    public string Expiry_date { get; set; } //String is de juiste datatype.
    //    public string CVV { get; set; }
    //    //TODO: Foreign key naar Users(User_id)
    //}

    //public class RateContext : DbContext
    //{
    //    public DbSet<User> Users { get; set; }
    //    public DbSet<Book> Books { get; set; } //niet klaar
    //}
    
//}