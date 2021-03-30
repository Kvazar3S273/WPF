using CatRenta.Domain;
using Microsoft.EntityFrameworkCore;
using System;

namespace CatRenta.EFData
{

    public class EFDataContext : DbContext
    {
        public DbSet<AppCat> Cats { get; set; }
        public DbSet<AppCatPrice> CatPrices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=91.238.103.51; Port=5743; Database=ba2hdb; Username=ba2h; Password=$544$B77w**G)K$t!ba2h22}");
        }
    }
}
