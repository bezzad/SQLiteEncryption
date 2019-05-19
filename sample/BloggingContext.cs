using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.SQLite
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        private SqliteConnection Connection { get; set; }
        private static string Password { get; set; } = "pass123";

        public BloggingContext()
        {
        }

        public BloggingContext(SqliteConnection sqliteConnection)
        {
            Connection = sqliteConnection;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Connection = InitializeSqliteConnection();
            optionsBuilder.UseSqlite(Connection);
        }

        // SQLCipher Encryption is applied to database using DBBrowser for SQLite.
        // DBBrowser for SQLite is free and open source tool to edit the SQLite files. 
        private static SqliteConnection InitializeSqliteConnection()
        {
            var connection = GetConnectionString();
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.Parameters.Clear();
                command.CommandText = $"PRAGMA key = '{Password}';";
                command.ExecuteNonQuery();

                return connection;
            }
        }

        public static void RemovePassword()
        {
            using (var connection = GetConnectionString())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.Parameters.Clear();
                    command.CommandText = $"PRAGMA key='{Password}';";
                    command.ExecuteNonQuery();

                    command.CommandText = "PRAGMA rekey='';";
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void ChangePassword(string newpass)
        {
            using (var connection = GetConnectionString())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.Parameters.Clear();
                    command.CommandText = $"PRAGMA key='{Password}';";
                    command.ExecuteNonQuery();

                    command.CommandText = $"PRAGMA rekey='{newpass}';";
                    command.ExecuteNonQuery();
                }
            }
        }


        private static SqliteConnection GetConnectionString()
        {
            return new SqliteConnection("Data Source=test.db");
        }
    }
}
