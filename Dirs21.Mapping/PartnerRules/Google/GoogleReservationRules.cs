using Dirs21.Mapping;
using Dirs21.MappingSystem.PartnerRules.Google;

namespace Dirs21.Mapping.PartnerRules;

/// <summary>
/// Google-specific business rules for reservations
/// </summary>
public class GoogleReservationRules : IPartnerRules<Google.Reservation>
{
    public void Apply(Google.Reservation target)
    {
        // Example: Google requires guest names in uppercase
        target.GuestFullName = target.GuestFullName.ToUpperInvariant();
        
        // Example: Google requires booking IDs to have a specific prefix
        if (!target.BookingId.StartsWith("GOOGLE-"))
        {
            target.BookingId = $"GOOGLE-{target.BookingId}";
        }
    }
    public bool Validate(Google.Reservation target, out List<string> errors)
    {
            var errorCodes = new List<GoogleReservationError>();

            if (string.IsNullOrWhiteSpace(target.BookingId))
                errorCodes.Add(GoogleReservationError.BookingIdRequired);

            if (string.IsNullOrWhiteSpace(target.GuestFullName))
                errorCodes.Add(GoogleReservationError.GuestFullNameRequired);

            if (target.DepartureDate <= target.ArrivalDate)
                errorCodes.Add(GoogleReservationError.DepartureBeforeArrival);

            var stayDuration =
                (target.DepartureDate - target.ArrivalDate).Days;

            if (stayDuration < 1)
                errorCodes.Add(GoogleReservationError.MinimumStayNotMet);

            errors = errorCodes
                .Select(e => e.ToMessage())
                .ToList();

            return errors.Count == 0;
    }
}
