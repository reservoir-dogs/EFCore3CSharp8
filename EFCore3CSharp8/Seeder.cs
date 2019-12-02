using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFCore3CSharp8
{
    public static class Seeder
    {
        public static void Seed(ApplicationDbContext applicationDbContext)
        {
            if (!applicationDbContext.Foos.Any())
            {
                using (var transaction = applicationDbContext.Database.BeginTransaction())
                {
                    var list1 = new List<string>();
                    for (int i = 0; i < 20; i++)
                    {
                        list1.Add(i.ToString());
                    }

                    var list2 = new List<string?>();
                    list2.Add(null);
                    for (int i = 0; i < 20; i++)
                    {
                        list2.Add(i.ToString());
                    }

                    var rand = new Random();

                    int j = 0;
                    while (j < 1000000)
                    {
                        var foo = new Foo
                        {
                            Required = list1[rand.Next(0, 19)],
                            Nullable = list2[rand.Next(0, 20)]
                        };

                        applicationDbContext.Foos.Add(foo);

                        if (j % 10000 == 0)
                            applicationDbContext.SaveChanges();

                        j++;
                    }

                    applicationDbContext.SaveChanges();
                    transaction.Commit();
                }
            }
        }
    }
}
