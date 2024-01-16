using StockControlApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Persistence.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().Property(e => e.Id).ValueGeneratedOnAdd();
        }
        public virtual DbSet<Product> Products => Set<Product>();
        public virtual DbSet<Movement> Movemments => Set<Movement>();
        public virtual DbSet<Notification> Notifications => Set<Notification>();
        public virtual DbSet<Chat> Chats => Set<Chat>();
        public virtual DbSet<Message> Messages => Set<Message>();

        public virtual DbSet<UserPushNotifications> PushNotifications => Set<UserPushNotifications>();

    }
}
