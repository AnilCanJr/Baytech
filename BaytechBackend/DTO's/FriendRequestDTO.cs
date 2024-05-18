using System;
namespace BaytechBackend.DTOs
{
	public class NotificationDTO
	{
        public int NotificationId { get; set; }
        public int NotificationTypeId { get; set; }
        public int? ClientUserId { get; set; }
        public int? TargetUserId { get; set; }
    }
}

