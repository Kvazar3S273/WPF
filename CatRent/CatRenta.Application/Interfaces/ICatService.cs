using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CatRenta.Application.Interfaces
{
    public delegate void InsertCatDelegate(int i);  // delegate
    public interface ICatService
    {
        //викликається коли додали один елемент
        event InsertCatDelegate EventInsertItem; // event
        /// <summary>
        /// Повертає кількість елементів БД
        /// </summary>
        /// <returns>Кількість елементів</returns>
        int Count();
        /// <summary>
        /// Додати множину елементів в БД
        /// </summary>
        /// <param name="count"></param>
        void InsertCats(int count, ManualResetEvent mrse);
        bool CanselAsyncMethod { get; set; }
        Task InsertCatsAsync(int count, ManualResetEvent mrse);

    }
}
