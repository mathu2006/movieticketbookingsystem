namespace MovieTicketBooking.DTOs.Screens
{
    public class CreateScreenRequestDto
    {
        public int TheatreId { get; set; }
        public string ScreenName { get; set; } = string.Empty;
    }
}
