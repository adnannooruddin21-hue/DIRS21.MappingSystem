using Dirs21.Mapping;

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
        errors = new List<string>();

        if (string.IsNullOrWhiteSpace(target.BookingId))
            errors.Add("BookingId is required for Google");

        if (string.IsNullOrWhiteSpace(target.GuestFullName))
            errors.Add("GuestFullName is required for Google");

        if (target.DepartureDate <= target.ArrivalDate)
            errors.Add("DepartureDate must be after ArrivalDate");

        // Google-specific: minimum stay requirement
        var stayDuration = (target.DepartureDate - target.ArrivalDate).Days;
        if (stayDuration < 1)
            errors.Add("Google requires minimum 1 night stay");

        return errors.Count == 0;
    }
}
