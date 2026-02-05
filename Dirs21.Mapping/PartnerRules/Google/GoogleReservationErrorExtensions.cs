namespace Dirs21.MappingSystem.PartnerRules.Google
{
    public static class GoogleReservationErrorExtensions
    {
        public static string ToMessage(this GoogleReservationError error)
        {
            return error switch
            {
                GoogleReservationError.BookingIdRequired =>
                    "BookingId is required for Google",

                GoogleReservationError.GuestFullNameRequired =>
                    "GuestFullName is required for Google",

                GoogleReservationError.DepartureBeforeArrival =>
                    "DepartureDate must be after ArrivalDate",

                GoogleReservationError.MinimumStayNotMet =>
                    "Google requires minimum 1 night stay",

                _ => "Unknown Google validation error"
            };
        }
    }
}
