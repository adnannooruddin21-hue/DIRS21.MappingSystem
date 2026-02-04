using System.Reflection;

namespace Dirs21.Mapping.Tests;

public class AutoRegistrationTests
{
    [Fact]
    public void Should_Auto_Register_All_Mappers_From_Assembly()
    {
        // Arrange
        var registry = new MapperRegistry();
        var assembly = Assembly.GetAssembly(typeof(Dirs21ToGoogleReservationMapper));

        // Act
        registry.RegisterFromAssembly(assembly!);

        // Assert
        var registeredMappers = registry.GetRegisteredMappers().ToList();
        Assert.NotEmpty(registeredMappers);
        Assert.Contains(registeredMappers, m => 
            m.Source == typeof(Dirs21.Reservation) && 
            m.Target == typeof(Google.Reservation));
        Assert.Contains(registeredMappers, m => 
            m.Source == typeof(Google.Reservation) && 
            m.Target == typeof(Dirs21.Reservation));
    }

    [Fact]
    public void Auto_Registered_Mappers_Should_Work_Correctly()
    {
        // Arrange
        var registry = new MapperRegistry();
        registry.RegisterFromAssembly(Assembly.GetAssembly(typeof(Dirs21ToGoogleReservationMapper))!);
        
        var handler = new MapHandler(registry);

        var source = new Dirs21.Reservation
        {
            ReservationId = "AUTO-123",
            CheckIn = new DateTime(2026, 5, 1),
            CheckOut = new DateTime(2026, 5, 5),
            GuestName = "Auto Test"
        };

        // Act
        var googleResult = handler.Map<Dirs21.Reservation, Google.Reservation>(source);

        // Assert
        Assert.Equal("AUTO-123", googleResult.BookingId);
        Assert.Equal("Auto Test", googleResult.GuestFullName);
    }

    [Fact]
    public void Should_Not_Register_Internal_Helper_Classes()
    {
        // Arrange
        var registry = new MapperRegistry();
        var assembly = Assembly.GetAssembly(typeof(MapperRegistry));

        // Act
        registry.RegisterFromAssembly(assembly!);

        // Assert - should only register public mapper classes
        var registeredMappers = registry.GetRegisteredMappers().ToList();
        foreach (var (source, target) in registeredMappers)
        {
            // Ensure no internal adapter/wrapper classes are registered
            Assert.DoesNotContain("Adapter", source.Name);
            Assert.DoesNotContain("Wrapper", source.Name);
            Assert.DoesNotContain("Adapter", target.Name);
            Assert.DoesNotContain("Wrapper", target.Name);
        }
    }

    [Fact]
    public void Auto_Registration_Should_Be_Idempotent()
    {
        // Arrange
        var registry = new MapperRegistry();
        var assembly = Assembly.GetAssembly(typeof(Dirs21ToGoogleReservationMapper));

        // Act
        registry.RegisterFromAssembly(assembly!);
        var firstCount = registry.GetRegisteredMappers().Count();

        // Registering same assembly again should throw due to duplicate prevention
        var exception = Assert.Throws<InvalidOperationException>(() => 
            registry.RegisterFromAssembly(assembly!));

        // Assert
        Assert.Contains("already registered", exception.Message);
    }

    [Fact]
    public void GetRegisteredMappers_Should_Return_All_Mappings()
    {
        // Arrange
        var registry = new MapperRegistry();
        registry.Register(new Dirs21ToGoogleReservationMapper());
        registry.Register(new GoogleToDirs21ReservationMapper());

        // Act
        var mappers = registry.GetRegisteredMappers().ToList();

        // Assert
        Assert.Equal(2, mappers.Count);
        Assert.All(mappers, mapper =>
        {
            Assert.NotNull(mapper.Source);
            Assert.NotNull(mapper.Target);
        });
    }
}
