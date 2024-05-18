using System;
using BaytechBackend.Entities;
using System.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BaytechBackend
{
	public class BaytechDbContext: IdentityDbContext<User,Role,int>
    {
		public BaytechDbContext(DbContextOptions<BaytechDbContext> options) :base(options)
		{
		}

		public DbSet<Call> Calls { get; set; }
        public DbSet<CallingStatus> CallingStatuses { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<Notification> Notificationes { get; set; }
        public DbSet<NotificationType> NotificationTypes { get; set; }
        public DbSet<Prefernce> Prefernces { get; set; }
       //  public DbSet<User> Users { get; set; }

    }
}
