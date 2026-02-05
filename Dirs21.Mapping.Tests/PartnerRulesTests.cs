using Dirs21.Mapping.PartnerRules;
using Dirs21.MappingSystem.PartnerRules.Google;

namespace Dirs21.Mapping.Tests;

public class PartnerRulesTests
{
    [Fact]
    public void GoogleRules_Should_Apply_Uppercase_Transformation()
    {
        // Arrange
        var rules = new GoogleReservationRules();
        var reservation = new Google.Reservation
        {
            BookingId = "G-123",
            ArrivalDate = DateTime.Today,
            DepartureDate = DateTime.Today.AddDays(2),
            GuestFullName = "john doe"
        };

        // Act
        rules.Apply(reservation);

        // Assert
        Assert.Equal("JOHN DOE", reservation.GuestFullName);
    }

    [Fact]
    public void GoogleRules_Should_Add_Prefix_To_BookingId()
    {
        // Arrange
        var rules = new GoogleReservationRules();
        var reservation = new Google.Reservation
        {
            BookingId = "123",
            ArrivalDate = DateTime.Today,
            DepartureDate = DateTime.Today.AddDays(2),
            GuestFullName = "Test User"
        };

        // Act
        rules.Apply(reservation);

        // Assert
        Assert.Equal("GOOGLE-123", reservation.BookingId);
    }

    [Fact]
    public void GoogleRules_Should_Not_Add_Prefix_If_Already_Present()
    {
        // Arrange
        var rules = new GoogleReservationRules();
        var reservation = new Google.Reservation
        {
            BookingId = "GOOGLE-123",
            ArrivalDate = DateTime.Today,
            DepartureDate = DateTime.Today.AddDays(2),
            GuestFullName = "Test User"
        };

        // Act
        rules.Apply(reservation);

        // Assert
        Assert.Equal("GOOGLE-123", reservation.BookingId);
    }

    [Fact]
    public void GoogleRules_Should_Validate_Valid_Reservation()
    {
        // Arrange
        var rules = new GoogleReservationRules();
        var reservation = new Google.Reservation
        {
            BookingId = "GOOGLE-123",
            ArrivalDate = DateTime.Today,
            DepartureDate = DateTime.Today.AddDays(2),
            GuestFullName = "Valid User"
        };

        // Act
        var isValid = rules.Validate(reservation, out var errors);

        // Assert
        Assert.True(isValid);
        Assert.Empty(errors);
    }

    [Theory]
    [InlineData("", "Test User")]
    [InlineData("G-123", "")]
    public void GoogleRules_Should_Fail_Validation_For_Missing_Required_Fields(string bookingId, string guestName)
    {
        // Arrange
        var rules = new GoogleReservationRules();
        var reservation = new Google.Reservation
        {
            BookingId = bookingId,
            ArrivalDate = DateTime.Today,
            DepartureDate = DateTime.Today.AddDays(2),
            GuestFullName = guestName
        };

        // Act
        var isValid = rules.Validate(reservation, out var errors);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void GoogleRules_Should_Fail_Validation_For_Invalid_Date_Range()
    {
        // Arrange
        var rules = new GoogleReservationRules();
        var reservation = new Google.Reservation
        {
            BookingId = "G-123",
            ArrivalDate = DateTime.Today,
            DepartureDate = DateTime.Today, // Same as arrival
            GuestFullName = "Test User"
        };

        // Act
        var isValid = rules.Validate(reservation, out var errors);

        // Assert
        Assert.False(isValid);
        Assert.Contains(GoogleReservationError.DepartureBeforeArrival.ToMessage(), errors);
    }

    [Fact]
    public void GoogleRules_Should_Fail_Validation_For_Missing_BookingId()
    {
        // Arrange
        var rules = new GoogleReservationRules();
        var reservation = new Google.Reservation
        {
            BookingId = "",
             ArrivalDate = DateTime.Today,
            DepartureDate = DateTime.Today.AddDays(2),
            GuestFullName = "Test User"
        };

        // Act
        var isValid = rules.Validate(reservation, out var errors);

        // Assert
        Assert.False(isValid);
        Assert.Contains(GoogleReservationError.BookingIdRequired.ToMessage(), errors);
    }

    [Fact]
    public void GoogleRules_Should_Fail_Validation_For_Missing_GuestName()
    {
        // Arrange
        var rules = new GoogleReservationRules();
        var reservation = new Google.Reservation
        {
            BookingId = "G-123",
            ArrivalDate = DateTime.Today,
            DepartureDate = DateTime.Today.AddDays(2),
            GuestFullName = ""
        };

        // Act
        var isValid = rules.Validate(reservation, out var errors);

        // Assert
        Assert.False(isValid);
        Assert.Contains(GoogleReservationError.GuestFullNameRequired.ToMessage(), errors);
    }

    [Fact]
    public void Partner_Rules_Can_Be_Applied_After_Mapping()
    {
        // Arrange
        var registry = new MapperRegistry();
        registry.Register(new Dirs21ToGoogleReservationMapper());
        var handler = new MapHandler(registry);
        var googleRules = new GoogleReservationRules();

        var source = new Dirs21.Reservation
        {
            ReservationId = "R-100",
            CheckIn = DateTime.Today,
            CheckOut = DateTime.Today.AddDays(3),
            GuestName = "test user"
        };

        // Act
        var googleReservation = handler.Map<Dirs21.Reservation, Google.Reservation>(source);
        googleRules.Apply(googleReservation);

        // Assert
        Assert.Equal("GOOGLE-R-100", googleReservation.BookingId);
        Assert.Equal("TEST USER", googleReservation.GuestFullName);
    }
}
