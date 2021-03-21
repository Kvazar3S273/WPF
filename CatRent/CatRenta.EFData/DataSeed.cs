using CatRenta.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatRenta.EFData
{
    public class DataSeed
    {
        public static void SeedDataAsync(EFDataContext context)
        {
            if (!context.Cats.Any())
            {
                var cats = new List<AppCat>
                                {
                                    new AppCat
                                        {
                                            Name = "Барсік",
                                            Gender=true,
                                            Birthday=new DateTime(2018, 5,23),
                                            Details="Стан критичний",
                                            Image="https://ichef.bbci.co.uk/news/800/cpsprodpb/14236/production/_104368428_gettyimages-543560762.jpg"
                                        },

                                };

                foreach (var cat in cats)
                {
                    context.Add(cat);
                    context.SaveChanges();
                }
            }
        }
    }
}
