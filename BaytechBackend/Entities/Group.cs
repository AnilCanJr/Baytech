using System;
namespace BaytechBackend.Entities
{
	public class Group
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? GroupImagePath { get; set; }
		public ICollection<GroupUser> GroupUsers { get; set; } = new List<GroupUser>();
	}
}

