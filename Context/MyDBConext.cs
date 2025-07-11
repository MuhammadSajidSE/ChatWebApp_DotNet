using ChatWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatWebApp.Context
{
    public class MyDBConext:DbContext
    {
        public MyDBConext(DbContextOptions<MyDBConext> options):base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Contacts> Contacts { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<Session> Sessions { get; set; }
    }
}
