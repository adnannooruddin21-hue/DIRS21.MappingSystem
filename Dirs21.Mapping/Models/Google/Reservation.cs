namespace Google;

public class Reservation
{
    public string BookingId { get; set; } = string.Empty;
    public DateTime ArrivalDate { get; set; }
    public DateTime DepartureDate { get; set; }
    public string GuestFullName { get; set; } = string.Empty;
}
