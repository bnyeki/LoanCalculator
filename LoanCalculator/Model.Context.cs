﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LoanCalculator
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class LoanDbEntities1 : DbContext
    {
        public LoanDbEntities1()
            : base("name=LoanDbEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<LoanSearchParameter> LoanSearchParameter { get; set; }
        public virtual DbSet<Period> Period { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
    }
}
