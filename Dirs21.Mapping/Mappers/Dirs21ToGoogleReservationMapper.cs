using Dirs21.Mapping;

public class Dirs21ToGoogleReservationMapper : IObjectMapper<Dirs21.Reservation, Google.Reservation>
{
    public Google.Reservation Map(Dirs21.Reservation source)
    {
        return new Google.Reservation
        {
            BookingId = source.ReservationId,
            ArrivalDate = source.CheckIn,
            DepartureDate = source.CheckOut,
            GuestFullName = source.GuestName
        };
    }
}
