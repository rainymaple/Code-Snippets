//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace STORI.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class StoriEntities : DbContext
    {
        public StoriEntities()
            : base("name=StoriEntities")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<ApiAppRegistration> ApiAppRegistrations { get; set; }
        public virtual DbSet<ApiUser> ApiUsers { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }
        public virtual DbSet<Objective> Objectives { get; set; }
        public virtual DbSet<School> Schools { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<UserAccount> UserAccounts { get; set; }
    }
}
