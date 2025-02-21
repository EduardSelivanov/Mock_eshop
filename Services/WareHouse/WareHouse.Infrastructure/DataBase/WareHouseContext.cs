using Microsoft.EntityFrameworkCore;
using WareHouse.Domain.Models;


namespace WareHouse.Infrastructure.DataBase
{
    internal class WareHouseContext:DbContext
    {
      

        public WareHouseContext(DbContextOptions<WareHouseContext> opts):base(opts)
        {
            
        }
        public DbSet<RackModel> RackTable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.AddInterceptors(new RackInterceptor());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RackModel>().HasKey(rm => rm.RackId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
