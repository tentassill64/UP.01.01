﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace up.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class UPEntities1 : DbContext
    {
        private static UPEntities1 _context;
        public UPEntities1()
            : base("name=UPEntities1")
        {
        }
        public static UPEntities1 GetContext()
        {
            if (_context == null)
                _context = new UPEntities1();
            return _context;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Agents> Agents { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<ProductSales> ProductSales { get; set; }
    }
}
