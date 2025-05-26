namespace KataSimpleAPI.Models
{
    public class BookingMessage
    {
        public int BookingId { get; set; }
        public int RoomId { get; set; }
        public int PersonId { get; set; }
        public DateTime BookingDate { get; set; }
        public int StartSlot { get; set; }
        public int EndSlot { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
