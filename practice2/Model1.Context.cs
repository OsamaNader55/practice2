﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace practice2
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class StoreEntities : DbContext
    {
        public StoreEntities()
            : base("name=StoreEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<branche_stores> branche_stores { get; set; }
        public virtual DbSet<branch> branches { get; set; }
        public virtual DbSet<order_details> order_details { get; set; }
        public virtual DbSet<order_kind> order_kind { get; set; }
        public virtual DbSet<order_master> order_master { get; set; }
        public virtual DbSet<permissions_groups> permissions_groups { get; set; }
        public virtual DbSet<permissions_screens> permissions_screens { get; set; }
        public virtual DbSet<product> products { get; set; }
        public virtual DbSet<products_category> products_category { get; set; }
        public virtual DbSet<unit> units { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<users_kind> users_kind { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
    }
}
