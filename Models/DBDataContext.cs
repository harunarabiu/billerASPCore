using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using FirstApp.Services;
using Refit;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FirstApp.Models
{
    public class DBDataContext : IdentityDbContext<ApplicationUser>
    {
        //private readonly UserManager<ApplicationUser> _userManager;

        public DBDataContext(DbContextOptions<DBDataContext> options) : base(options)
        {
            Database.GetPendingMigrations();
            Database.Migrate();
        }
        //public DbSet<Account> Accounts { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServicePlan> ServicePlans { get; set; }
        public DbSet<PaymentPlan> PaymentPlans { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<ReservedAccount> ReservedAccounts { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<BankTransfer> BankTransfers { get; set; }
        public DbSet<CustomerPaymentPlan> CustomerPaymentPlans { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<ServiceType>()
            //.HasData(
            //    new ServiceType
            //    {
            //        Name = "Electricity",
            //        Key = "Electricity"
            //    }
            //);
            //builder.Entity<Service>()
            //.HasData(
            //    new Service
            //    {
            //        Name = "KEDCO",
            //        Key = "kedco",
            //        ServiceTypeId = 1,

            //    }
            //);
            //builder.Entity<ServicePlan>()
            //.HasData(new ServicePlan
            //{
            //    Name = "Prepaid",
            //    Key = "prepaid",
            //    ServiceId = 1,
            //    Price = 0,
            //    IsDefault = true,
            //    CommissionType = "Variable",
            //    Commission = 3.5,
            //    ConvienceFee = 50

            //}
            //);

            //builder.Entity<Transaction>()
            //builder.Entity<Transaction>()
            //    .Property(b => b.RawData)
            //    .HasConversion(
            //        v => JsonConvert.SerializeObject(v),
            //        v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v));
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }


        public override int SaveChanges()
        {
            //CreateWallet();
            AddTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            //CreateWallet();
            AddTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }



        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseModel && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow; // current datetime

                if (entity.State == EntityState.Added)
                {
                    ((BaseModel)entity.Entity).CreatedAt = now;
                }
                ((BaseModel)entity.Entity).UpdatedAt = now;
            }
        }


        //private void CreateReservedAccount()
        //{
        //    var entities = ChangeTracker.Entries()
        //        .Where(x => x.Entity is Wallet && x.State == EntityState.Added);

        //    var cachedToken = String.Empty;

        //    var api = RestService.For<IMonnifyClient>("kedco", new RefitSettings {

        //        AuthorizationHeaderValueGetter = () => Task.FromResult(cachedToken)

        //    });

        //    api.CreateReservedAccountAsync();

        //    foreach (var entity in entities)
        //    {
        //        var now = DateTime.UtcNow; // current datetime

        //        if (entity.State == EntityState.Added)
        //        {
        //            ((Wallet)entity.Entity).Id = new Guid();
        //        }

        //    }
        //}

        //private async void CreateWallet()
        //{
        //    var entities = ChangeTracker.Entries()
        //        .Where(x => x.Entity is ApplicationUser && x.State == EntityState.Added);

        //    foreach (var entity in entities)
        //    {

        //        if (entity.State == EntityState.Added)
        //        {
        //            var UserId = ((ApplicationUser)entity.Entity).Id;
        //            ApplicationUser User = await ApplicationUser.get

        //            var wallet = new Wallet
        //            {
        //                Balance = 0.0,
        //                BookBalance = 0.0,
        //                User = User,
        //            };

        //            this.Wallets.Add(wallet);

        //        }

        //    }
        //}



    }
}
