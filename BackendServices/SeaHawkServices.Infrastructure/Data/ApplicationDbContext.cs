using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaHawkServices.Domain.Entities;
using Data;
using SeaHawkServices.Domain.StoredProcedures;
using System.Reflection.Emit;



namespace SeaHawkServices.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<AnalysisResult> AnalysisResult { get; set; }
        public DbSet<VesselDetail> VesselDetail { get; set; }
        public DbSet<VesselUser> VesselUser { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<CompanyUser> CompanyUser { get; set; }

        public DbSet<SmtpSetting> SmtpSettings { get; set; }
        //public DbSet<SampleCollection> SampleCollection { get; set; }
        //public DbSet<SampCollection> SampCollection { get; set; }
        //public DbSet<MSCAnalysis> MSCAnalysis { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<NewsFeed> NewsFeed { get; set; }
        public DbSet<SamplingKit> SamplingKit { get; set; }
        //public DbSet<AccountHistoryWithVessel> AccountHistoryWithVessel { get; set; }
        public DbSet<SampleCollections> SampleCollections { get; set; }
        //public DbSet<Tbl_SampleNumberName> SampleNumberNames { get; set; }
        public DbSet<Career> Career { get; set; }
        public DbSet<PDF> PDF { get; set; }
        //public DbSet<LiveDataRow> LiveDataRows { get; set; } = null!;
        public DbSet<RequestHistory> RequestHistory { get; set; }
        public DbSet<UserLoginHistory> UserLoginHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);
            mb.Entity<AccountHistoryWithVessel>().HasNoKey();
            // Your custom configs
            mb.Entity<DistillateRow>()
              .HasNoKey()
              .ToView((string)null); 
            mb.Entity<ResidualRow>()
              .HasNoKey()
              .ToView((string)null); 
            mb.Entity<LiveDataRow>()
              .HasNoKey()
              .ToView((string)null);

            var e = mb.Entity<SamplingKit>();

            // Any property that can be NULL in the DB must be optional to EF:
            e.Property(p => p.VesselName).IsRequired(false);
            e.Property(p => p.RequestorName).IsRequired(false);
            e.Property(p => p.RequestorEmail).IsRequired(false);
            e.Property(p => p.VPSCustomerName).IsRequired(false);
            e.Property(p => p.Street).IsRequired(false);
            e.Property(p => p.City).IsRequired(false);
            e.Property(p => p.CompanyName).IsRequired(false);
            e.Property(p => p.PersonToContactTelNo).IsRequired(false);
            e.Property(p => p.BillingCompanyName).IsRequired(false);
            e.Property(p => p.BillingAddressLine1).IsRequired(false);
            e.Property(p => p.BillingPostalCode).IsRequired(false);
            e.Property(p => p.DeliveryEmail).IsRequired(false);
            e.Property(p => p.EmailCC).IsRequired(false);
            e.Property(p => p.BillingAddressLine2).IsRequired(false);
            e.Property(p => p.BillingAddressLine3).IsRequired(false);

            // DeliveryDeadline is a DateTime?, set optional if DB has NULLs
            e.Property(p => p.DeliveryDeadline).IsRequired(false);
        }
    }
}
