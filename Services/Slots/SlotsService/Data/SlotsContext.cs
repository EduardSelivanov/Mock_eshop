using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SlotsService.Models;

namespace SlotsService.Data
{
    public class SlotsContext:DbContext
    {
        public DbSet<SlotModel> SlotsTable { get; set;}

        public SlotsContext(DbContextOptions opts) : base(opts){ }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SlotModel>().HasKey(x => x.SlotId);
        }
    }
}
