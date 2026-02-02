using Dirs21.Mapping;

public class GoogleToDirs21ReservationMapper : IObjectMapper<Google.Reservation, Dirs21.Reservation>
{
    public Dirs21.Reservation Map(Google.Reservation source)
    {
        return new Dirs21.Reservation
        {
            ReservationId = source.BookingId,
            CheckIn = source.ArrivalDate,
            CheckOut = source.DepartureDate,
            GuestName = source.GuestFullName
        };
    }
}
