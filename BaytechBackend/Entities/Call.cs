using System;
namespace BaytechBackend.Entities
{
	public class Call
	{
		public int Id { get; set; }
		public User ?ClientUser { get; set; }
		public int ?ClientUserId { get; set; }
        public User ?TargetUser { get; set; }
        public int? TargetUserId { get; set; }
		public DateTime StartTime { get; set; }
        public CallingStatus ?CallingStatus { get; set; }
        public int CallingStatusId { get; set; }
    }
}

