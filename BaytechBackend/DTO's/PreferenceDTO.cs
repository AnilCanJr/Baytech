using System;
namespace BaytechBackend.DTOs
{
	public class PreferenceDTO
	{
		public int UserId { get; set; }
        public bool LastSeenOn { get; set; }
        public bool DarkMode { get; set; }
        public bool PrivateProfile { get; set; }
    }
}

