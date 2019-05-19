using System;

namespace ConsoleApp.SQLite
{
    public class Program
    {
        public static void Main()
        {
            // SQLCipher Encryption is applied to database using DBBrowser for SQLite.
            // DBBrowser for SQLite is free and open source tool to edit the SQLite files. 
            using (var db = new BloggingContext())
            {
                db.Database.EnsureCreated(); // create new db, if that not exist!

                db.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);

                Console.WriteLine("All blogs in database:");
                foreach (var blog in db.Blogs)
                {
                    Console.WriteLine(" - {0}", blog.Url);
                }
            }

            Console.ReadKey();
            Console.WriteLine("Press any key to remove db password...");
//            BloggingContext.ChangePassword("new pass");
            BloggingContext.RemovePassword();
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();

        }
    }
}