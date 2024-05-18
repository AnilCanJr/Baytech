using System;
namespace BaytechBackend.Entities
{
	public class Friend
	{
		public int Id { get; set; }
		public User ?UserOne { get; set; }
		public int? UserOneId { get; set; }
        public User? UserTwo { get; set; }
        public int? UserTwoId { get; set; }
    }
}

