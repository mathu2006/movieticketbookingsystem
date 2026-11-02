namespace MovieTicketBooking.DTOs.Screens
{
    public class UpdateScreenRequestDto
    {
        public int TheatreId { get; set; }
        public string ScreenName { get; set; } = string.Empty;
    }
}
