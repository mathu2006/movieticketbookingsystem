
namespace MovieTicketBooking.DTOs.Seats
{
    public class BulkCreateSeatsRequestDto
    {
        public int ScreenId { get; set; }

        public List<CreateSeatRequestDto> Seats { get; set; } = new List<CreateSeatRequestDto>();
    }
}
