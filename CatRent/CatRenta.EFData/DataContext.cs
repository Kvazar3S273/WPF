using CatRenta.Domain;
using Microsoft.EntityFrameworkCore;
using System;

namespace CatRenta.EFData
{

    public class EFDataContext : DbContext
    {
        public DbSet<AppCat> Cats { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=91.238.103.51;Port=5743;Database=dbcatrent;Username=usercatrent;Password=$544$rrxlleo(ll3237sdfBG)K$t!Ube22}xk");
        }
    }
}
