﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace webtest.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DatabaseEntities1 : DbContext
    {
        public DatabaseEntities1()
            : base("name=DatabaseEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Creditcard_details> Creditcard_details { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Favorite> Favorites { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
    }
}
