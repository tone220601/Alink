using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Portal.Models
{
    public class AlinkContext : DbContext
    {
        public AlinkContext()
            : base("AlinkContextTest")
        {
           
            Database.SetInitializer<AlinkContext>(new CreateDatabaseIfNotExists<AlinkContext>());
        }


        public DbSet<Client> Clients { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<ModeOfTransport> ModeOfTravels { get; set; }
        public DbSet<RouteOfTravel> RouteOfTravels { get; set; }
        public DbSet<RequestForQuotation> RequestForQuotations { get; set; }

        //public DbSet<ModesOfTravel>         ModesOfTravel           { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>().HasMany<ClientSchemes>(c => c.ClientSchemes)
                .WithRequired(r => r.Client).HasForeignKey(f => f.ClientId);

            modelBuilder.Entity<ClientSchemes>().HasMany<ModeOfTransport>(c => c.ModeOfTransports)
                .WithRequired(r => r.ClientScheme).HasForeignKey(f => f.ClientSchemeId);

            modelBuilder.Entity<Customer>().HasMany<Employer>(c => c.Employer)
                .WithRequired(r => r.Customer).HasForeignKey(f => f.CustomerId);

            modelBuilder.Entity<Customer>().HasMany(c => c.RouteOfTravels)
                .WithRequired(r => r.Customer).HasForeignKey(f => f.CustomerId);

            modelBuilder.Entity<ModeOfTransport>().HasMany(c => c.RouteOfTravel)
                .WithRequired(r => r.ModeOfTravel).HasForeignKey(f => f.ModeOfTransportId);

            modelBuilder.Entity<RequestForQuotation>().HasRequired(r => r.RouteOfTravel);

        }
    }
}