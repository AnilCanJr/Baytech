using System;
namespace BaytechBackend.Entities
{
	public class Chat
	{
		public int Id { get; set; }
        public string SenderUsername { get; set; }
        public string ReceiverUsername { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

