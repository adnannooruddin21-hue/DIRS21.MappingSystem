namespace Dirs21.Mapping.Tests;

public class ExtensibilityTests
{
    [Fact]
    public void Registry_Prevents_Duplicate_Mapper_Registration()
    {
        // Arrange
        var registry = new MapperRegistry();
        registry.Register(new Dirs21ToGoogleReservationMapper());

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            registry.Register(new Dirs21ToGoogleReservationMapper()));
        
        Assert.Contains("already registered", exception.Message);
    }
}
