using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.DBcontext;


namespace Webshop.Models //TODO: Foreign keys & correcte constraints. Ook datatype check just to be sure
{
    public class Book //PK -> ISBN
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public DateTime Date { get; set; } //Is hier een betere datatype voor of iets dergelijks? Misschien een dag,maand,jaar tuple?
        public int Quantity { get; set; }
        public byte[] Image { get; set; } //https://stackoverflow.com/questions/42183640/show-image-from-database-in-mvc
        [Key]
        public string ISBN { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
    }

    public class Orders //PK -> Order_id
    {
        [Key]
        public int Order_id { get; set; }
        public double Amount { get; set; }
        public string Order_status { get; set; }
        public int PaymentId { get; set; }//Adds Foreign key to Payment(payment_id) http://www.entityframeworktutorial.net/code-first/configure-one-to-one-relationship-in-code-first.aspx
        public virtual Payment Payment { get; set; } //Ik twijfel of dit goed is 
        //TODO: Add Foreign key to Shopping_cart(cart_id)

    }

    public class Payment //PK -> Payment_id
    {
        [Key]
        public int Payment_id { get; set; } //In de sql code staat er dat het varchar is, dan vgm is de juiste c# equivalent: string
        public DateTime payment_date { get; set; }
        public double Amount { get; set; }
        public int OrderId { get; set; }//Adds Foreign key naar Orders(Order_id)
        public virtual Orders Orders { get; set; }
    }

    public class Wishlist //PK -> List_id
    {
        [Key]
        public string List_id { get; set; }
        public string user_id { get; set; }
    }

    public class Creditcard_details //PK -> CC_number
    {
        [Key]
        public string CC_number { get; set; }
        public string Owner_name { get; set; }
        public string Expiry_date { get; set; } //String is de juiste datatype.
        public string CVV { get; set; }
        //TODO: Foreign key naar Users(User_id)
    }

    public class RateContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; } //niet klaar
    }
    //TODO: Table voor de relatie tussen User en Book 'Rates'
    //TODO: Table voor de relatie tussen Shopping_cart en Book 'Shopping_cart_contains'
    //TODO: Table voor de relatie tussen Wishlist en Book 'Wishlist_contains'
}