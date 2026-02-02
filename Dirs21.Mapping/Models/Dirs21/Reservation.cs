namespace Dirs21;

public class Reservation
{
    public string ReservationId { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public string GuestName { get; set; } = string.Empty;
}
