using System;
namespace BaytechBackend.Entities
{
	public class Notification
	{
		public int Id { get; set; }
        public User ?ClientUser { get; set; }
        public int? ClientUserId { get; set; }
        public User ?TargetUser { get; set; }
        public int? TargetUserId { get; set; }
        public NotificationType ?NotificationType { get; set; }
        public int NotificationTypeId { get; set; }
        public bool Done { get; set; } = false;
    }
}

