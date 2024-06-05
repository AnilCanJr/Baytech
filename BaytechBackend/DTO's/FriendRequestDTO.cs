using System;
namespace BaytechBackend.DTOs
{
	public class NotificationDTO
	{
    
        public int NotificationTypeId { get; set; } = 1;
        public int? ClientUserId { get; set; }
        public int? TargetUserId { get; set; }
    }
}

