using System;
namespace BaytechBackend.Entities
{
	public class Prefernce
	{

		public int Id { get; set; }
        public User ?User { get; set; }
        public int? UserId { get; set; }
        public bool DarkMode { get; set; } = true;
        public bool LastSeenOn { get; set; } = true;
        public bool PrivateProfile { get; set; } = true;

    }
}

