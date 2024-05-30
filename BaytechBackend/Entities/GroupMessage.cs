namespace BaytechBackend.Entities
{
    public class GroupMessage
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string SenderUsername { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public Group Group { get; set; }
    }
}
