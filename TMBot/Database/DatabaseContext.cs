using System.Data.Entity;
using TMBot.Models;

namespace TMBot.Database
{
    public class DatabaseContext : DbContext
    {
         public DbSet<Item> Items { get; set; }
    }
}