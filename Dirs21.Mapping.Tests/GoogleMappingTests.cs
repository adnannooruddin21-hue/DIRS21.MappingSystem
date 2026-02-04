namespace Dirs21.Mapping.Tests;

public class GoogleMappingTests
{
    [Fact]
    public void Should_Map_Dirs21_Reservation_To_Google_Reservation()
    {
        // Arrange
        var registry = new MapperRegistry();
        registry.Register(new Dirs21ToGoogleReservationMapper());

        var handler = new MapHandler(registry);

        var source = new Dirs21.Reservation
        {
            ReservationId = "R123",
            CheckIn = new DateTime(2026, 1, 10),
            CheckOut = new DateTime(2026, 1, 12),
            GuestName = "Adnan"
        };

        // Act
        var result = handler.Map<Dirs21.Reservation, Google.Reservation>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("R123", result.BookingId);
        Assert.Equal(new DateTime(2026, 1, 10), result.ArrivalDate);
        Assert.Equal(new DateTime(2026, 1, 12), result.DepartureDate);
        Assert.Equal("Adnan", result.GuestFullName);
    }

    [Fact]
    public void Should_Map_Google_Reservation_To_Dirs21_Reservation()
    {
        // Arrange
        var registry = new MapperRegistry();
        registry.Register(new GoogleToDirs21ReservationMapper());

        var handler = new MapHandler(registry);

        var source = new Google.Reservation
        {
            BookingId = "G-456",
            ArrivalDate = new DateTime(2026, 2, 15),
            DepartureDate = new DateTime(2026, 2, 20),
            GuestFullName = "Jane Smith"
        };

        // Act
        var result = handler.Map<Google.Reservation, Dirs21.Reservation>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("G-456", result.ReservationId);
        Assert.Equal(new DateTime(2026, 2, 15), result.CheckIn);
        Assert.Equal(new DateTime(2026, 2, 20), result.CheckOut);
        Assert.Equal("Jane Smith", result.GuestName);
    }

    [Fact]
    public void Should_Preserve_Data_In_Round_Trip_Mapping()
    {
        // Arrange
        var registry = new MapperRegistry();
        registry.Register(new Dirs21ToGoogleReservationMapper());
        registry.Register(new GoogleToDirs21ReservationMapper());

        var handler = new MapHandler(registry);

        var original = new Dirs21.Reservation
        {
            ReservationId = "R999",
            CheckIn = new DateTime(2026, 3, 1),
            CheckOut = new DateTime(2026, 3, 5),
            GuestName = "Test User"
        };

        // Act
        var googleVersion = handler.Map<Dirs21.Reservation, Google.Reservation>(original);
        var roundTrip = handler.Map<Google.Reservation, Dirs21.Reservation>(googleVersion);

        // Assert
        Assert.Equal(original.ReservationId, roundTrip.ReservationId);
        Assert.Equal(original.CheckIn, roundTrip.CheckIn);
        Assert.Equal(original.CheckOut, roundTrip.CheckOut);
        Assert.Equal(original.GuestName, roundTrip.GuestName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("Very Long Name That Exceeds Normal Length Expectations For Testing")]
    public void Should_Handle_Various_Guest_Name_Formats(string guestName)
    {
        // Arrange
        var registry = new MapperRegistry();
        registry.Register(new Dirs21ToGoogleReservationMapper());

        var handler = new MapHandler(registry);

        var source = new Dirs21.Reservation
        {
            ReservationId = "R100",
            CheckIn = DateTime.Today,
            CheckOut = DateTime.Today.AddDays(1),
            GuestName = guestName
        };

        // Act
        var result = handler.Map<Dirs21.Reservation, Google.Reservation>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(guestName, result.GuestFullName);
    }
}
