using System;
using Microsoft.AspNetCore.Identity;

namespace BaytechBackend.Entities
{
	public class User:IdentityUser<int>
	{
		public DateTime LastSeen { get; set; }
        public bool IsOnline { get; set; }
    }
}

