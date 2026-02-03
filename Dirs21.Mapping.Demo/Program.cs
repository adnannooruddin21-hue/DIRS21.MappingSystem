using System.Reflection;
using Dirs21.Mapping;
using Dirs21.Mapping.PartnerRules;

Console.WriteLine("=== Dirs21 Mapping System Demo ===");
Console.WriteLine();

// Initialize the mapping system
var registry = new MapperRegistry();
var mapHandler = new MapHandler(registry);

// Auto-registration
Console.WriteLine("Auto-registering mappers from assembly...");
registry.RegisterFromAssembly(Assembly.GetAssembly(typeof(Dirs21ToGoogleReservationMapper))!);
Console.WriteLine($"    Registered {registry.GetRegisteredMappers().Count()} mapper(s)");
Console.WriteLine();

// Create a sample reservation
var dirs21Reservation = new Dirs21.Reservation
{
    ReservationId = "R-1001",
    CheckIn = new DateTime(2026, 3, 10),
    CheckOut = new DateTime(2026, 3, 12),
    GuestName = "Adnan"
};

Console.WriteLine("Original Dirs21 Reservation:");
Console.WriteLine($"    ReservationId: {dirs21Reservation.ReservationId}");
Console.WriteLine($"    CheckIn: {dirs21Reservation.CheckIn:yyyy-MM-dd}");
Console.WriteLine($"    CheckOut: {dirs21Reservation.CheckOut:yyyy-MM-dd}");
Console.WriteLine($"    GuestName: {dirs21Reservation.GuestName}");
Console.WriteLine();

//Type-safe generic mapping
Console.WriteLine("Mapping to Google format...");
var googleReservation = mapHandler.Map<Dirs21.Reservation, Google.Reservation>(dirs21Reservation);
Console.WriteLine($"    BookingId: {googleReservation.BookingId}");
Console.WriteLine($"    ArrivalDate: {googleReservation.ArrivalDate:yyyy-MM-dd}");
Console.WriteLine($"    DepartureDate: {googleReservation.DepartureDate:yyyy-MM-dd}");
Console.WriteLine($"    GuestFullName: {googleReservation.GuestFullName}");
Console.WriteLine();

//Partner-specific rules
Console.WriteLine("Applying Google-specific partner rules...");
var googleRules = new GoogleReservationRules();
googleRules.Apply(googleReservation);
Console.WriteLine($"    BookingId (with prefix): {googleReservation.BookingId}");
Console.WriteLine($"    GuestFullName (uppercase): {googleReservation.GuestFullName}");
Console.WriteLine();

// Validate with partner rules
Console.WriteLine("Validating with Google rules...");
if (googleRules.Validate(googleReservation, out var errors))
{
    Console.WriteLine("     Validation passed!");
}
else
{
    Console.WriteLine("    ✗ Validation failed:");
    foreach (var error in errors)
    {
        Console.WriteLine($"      - {error}");
    }
}
Console.WriteLine();

//Bi-directional mapping
Console.WriteLine("Mapping back to Dirs21 format...");
var mappedBack = mapHandler.Map<Google.Reservation, Dirs21.Reservation>(googleReservation);
Console.WriteLine($"    ReservationId: {mappedBack.ReservationId}");
Console.WriteLine($"    CheckIn: {mappedBack.CheckIn:yyyy-MM-dd}");
Console.WriteLine($"    CheckOut: {mappedBack.CheckOut:yyyy-MM-dd}");
Console.WriteLine($"    GuestName: {mappedBack.GuestName}");
Console.WriteLine();

//Collection Mapping
Console.WriteLine("Collection Mapping Demo");
var reservations = new List<Dirs21.Reservation>
{
    new() 
    { 
        ReservationId = "R-2001", 
        CheckIn = new DateTime(2026, 4, 1),
        CheckOut = new DateTime(2026, 4, 3),
        GuestName = "Alice Johnson" 
    },
    new() 
    { 
        ReservationId = "R-2002", 
        CheckIn = new DateTime(2026, 4, 5),
        CheckOut = new DateTime(2026, 4, 7),
        GuestName = "Bob Williams" 
    },
    new() 
    { 
        ReservationId = "R-2003", 
        CheckIn = new DateTime(2026, 4, 10),
        CheckOut = new DateTime(2026, 4, 12),
        GuestName = "Carol Martinez" 
    }
};
Console.WriteLine($"    Mapping {reservations.Count} reservations to Google format...");
var googleReservations = mapHandler.MapCollection<Dirs21.Reservation, Google.Reservation>(reservations);
foreach (var res in googleReservations)
{
    Console.WriteLine($"      → {res.BookingId}: {res.GuestFullName} ({res.ArrivalDate:yyyy-MM-dd} to {res.DepartureDate:yyyy-MM-dd})");
}
Console.WriteLine();

//Collection Mapping with Rules
Console.WriteLine("Collection Mapping with Rules Applied");
var googleReservationsWithRules = mapHandler.MapCollection(reservations, googleRules);
foreach (var res in googleReservationsWithRules)
{
    Console.WriteLine($"      → {res.BookingId}: {res.GuestFullName}");
}
Console.WriteLine();

//MapAndValidate
Console.WriteLine("MapAndValidate Demo");
var testReservation = new Dirs21.Reservation
{
    ReservationId = "R-3001",
    CheckIn = new DateTime(2026, 5, 1),
    CheckOut = new DateTime(2026, 5, 3),
    GuestName = "David Brown"
};
Console.WriteLine("    Mapping and validating in one operation...");
var result = mapHandler.MapAndValidate(testReservation, new GoogleReservationRules());
Console.WriteLine($"    Valid: {result.IsValid}");
if (result.IsValid)
{
    Console.WriteLine($"    Result: {result.Value.BookingId} - {result.Value.GuestFullName}");
}
else
{
    Console.WriteLine("    Errors:");
    result.Errors.ForEach(e => Console.WriteLine($"      - {e}"));
}
Console.WriteLine();

//MapAndValidate with invalid data
Console.WriteLine("MapAndValidate with Invalid Data");
var invalidReservation = new Dirs21.Reservation
{
    ReservationId = "",  // Invalid
    CheckIn = new DateTime(2026, 5, 10),
    CheckOut = new DateTime(2026, 5, 8),  // Invalid: before check-in
    GuestName = ""  // Invalid
};
var invalidResult = mapHandler.MapAndValidate(invalidReservation, new GoogleReservationRules());
Console.WriteLine($"    Valid: {invalidResult.IsValid}");
if (!invalidResult.IsValid)
{
    Console.WriteLine($"    Found {invalidResult.Errors.Count} validation error(s):");
    invalidResult.Errors.ForEach(e => Console.WriteLine($"      - {e}"));
}
Console.WriteLine();


Console.WriteLine();
Console.WriteLine("Demo completed successfully!");
Console.WriteLine();