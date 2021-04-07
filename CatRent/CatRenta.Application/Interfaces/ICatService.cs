using System;
using System.Collections.Generic;
using System.Text;

namespace CatRenta.Application.Interfaces
{
    public interface ICatService
    {
        /// <summary>
        /// Повертає кількість елементів БД
        /// </summary>
        /// <returns>Кількість елементів</returns>
        int Count();
        /// <summary>
        /// Додати множину елементів в БД
        /// </summary>
        /// <param name="count"></param>
        void InsertCats(int count);
    }
}
