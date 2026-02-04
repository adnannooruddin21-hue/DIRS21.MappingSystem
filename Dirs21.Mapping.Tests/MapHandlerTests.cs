namespace Dirs21.Mapping.Tests;

public class MapHandlerTests
{
    [Fact]
    public void Should_Throw_When_Generic_Mapper_Not_Found()
    {
        // Arrange
        var registry = new MapperRegistry();
        var handler = new MapHandler(registry);
        var reservation = new Dirs21.Reservation();

        // Act & Assert
        var exception = Assert.Throws<NotSupportedException>(() =>
            handler.Map<Dirs21.Reservation, Google.Reservation>(reservation)
        );

        Assert.Contains("No mapper found", exception.Message);
    }

    [Fact]
    public void Generic_Map_Should_Return_Correct_Type()
    {
        // Arrange
        var registry = new MapperRegistry();
        registry.Register(new Dirs21ToGoogleReservationMapper());
        var handler = new MapHandler(registry);

        var source = new Dirs21.Reservation
        {
            ReservationId = "TYPE-TEST",
            CheckIn = DateTime.Today,
            CheckOut = DateTime.Today.AddDays(1),
            GuestName = "Type Test"
        };

        // Act
        var result = handler.Map<Dirs21.Reservation, Google.Reservation>(source);

        // Assert
        Assert.IsType<Google.Reservation>(result);
        Assert.NotNull(result);
    }

}
