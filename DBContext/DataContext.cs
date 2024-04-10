using CloudMining.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CloudMining.DBContext
{
    public class DataContext : IdentityDbContext<User, Role, Guid>
    {
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<PaymentShare> PaymentShares { get; set; }
        public DbSet<ShareablePayment> ShareablePayments { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}
