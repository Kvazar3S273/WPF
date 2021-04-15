using CatRenta.Application.Interfaces;
using CatRenta.Domain;
using CatRenta.EFData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CatRenta.Infrastructure.Services
{
    public class CatService : ICatService
    {
        private readonly EFDataContext _context;
        public CatService()
        {
            _context = new EFDataContext();
        }
        // Відміна асинхронного метода
        public bool CanselAsyncMethod { get; set; }

        public event InsertCatDelegate EventInsertItem;

        // Повертаємо кількість котів у БД
        public int Count()
        {
            return _context.Cats.Count();
        }

        // Метод додавання групи котів у БД
        public void InsertCats(int count, ManualResetEvent mrse)
        {
            // Починаємо фіксувати час відліку 
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Підраховуємо скільки котів встигли добавити в базу
            List<AppCat> hasAdded = new List<AppCat>();

            for (int i = 0; i < count; i++)
            {
                mrse.WaitOne();
                // Робимо відміну при натисканні кнопки Cancel
                if (CanselAsyncMethod)
                {
                    CanselAsyncMethod = false;
                    break;
                }
                // Додаємо котів у базу
                AppCat appCat = new AppCat
                {
                    Name = "Cat" + i,
                    Birthday = DateTime.Now,
                    Details = "good cat",
                    Gender = true,
                    Image = "33445.jpg"
                };
                // І додаємо доданих котів у ліст
                hasAdded.Add(appCat);
                
                EventInsertItem?.Invoke(i + 1);
                
                //Console.WriteLine("Insert cat"+appCat.Id);

                // Якщо задана кількість котів співпадає з кількістю котів у лісті, то зберігаємо зміни
            if (hasAdded.Count == count)
                foreach (var item in hasAdded)
                {
                    _context.Cats.Add(item);
                    _context.SaveChanges();
                }
            }
            // Підраховуємо час, який був необхідний на додавання котів у базу
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            //Console.WriteLine("Час додавання котів:" + elapsedTime);
        }

        public Task InsertCatsAsync(int count, ManualResetEvent mrse)
        {
            return Task.Run(() => InsertCats(count, mrse));
        } 
    }
}
