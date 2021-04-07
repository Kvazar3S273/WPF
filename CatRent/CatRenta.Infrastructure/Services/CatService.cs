using CatRenta.Application.Interfaces;
using CatRenta.Domain;
using CatRenta.EFData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace CatRenta.Infrastructure.Services
{
    public class CatService : ICatService
    {
        private readonly EFDataContext _context;
        public CatService()
        {
            _context = new EFDataContext();
        }

        public int Count()
        {
            return _context.Cats.Count();
        }

        public void InsertCats(int count)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < count; i++)
            {
                AppCat appCat = new AppCat
                {
                    Name = "Cat" + i,
                    Birthday = DateTime.Now,
                    Details = "good cat",
                    Gender = true,
                    Image = "Image"
                };
                _context.Cats.Add(appCat);
                _context.SaveChanges();
                Console.WriteLine("Insert cat"+appCat.Id);
            }
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("Час додавання котів:" + elapsedTime);
        }
    }
}
