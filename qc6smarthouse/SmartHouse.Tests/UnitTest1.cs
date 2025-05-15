using Xunit;
using SmartHomeSystem;

public class SmartHomeTests
{
    [Fact]
    public void ActivateSystem_WhenInactive_ActivatesSystem()
    {
        // Arrange
        var smartHome = new SmartHome();

        // Act
        bool result = smartHome.ActivateSystem();

        // Assert
        Assert.True(result);
        Assert.True(smartHome.IsSystemActive);
    }

    [Fact]
    public void ActivateSystem_WhenActive_ReturnsFalse()
    {
        var smartHome = new SmartHome();
        smartHome.ActivateSystem(); // Активируем первый раз

        bool result = smartHome.ActivateSystem(); // Пытаемся активировать снова

        Assert.False(result);
    }

    [Fact]
    public void DeactivateSystem_WhenActiveAndSecurityDisarmed_Deactivates()
    {
        var smartHome = new SmartHome();
        smartHome.ActivateSystem();

        bool result = smartHome.DeactivateSystem();

        Assert.True(result);
        Assert.False(smartHome.IsSystemActive);
    }

    [Fact]
    public void DeactivateSystem_WhenSecurityArmed_ReturnsFalse()
    {
        var smartHome = new SmartHome();
        smartHome.ActivateSystem();
        smartHome.ToggleSecurity(); // Включаем охрану

        bool result = smartHome.DeactivateSystem();

        Assert.False(result);
        Assert.True(smartHome.IsSystemActive);
    }

    [Fact]
    public void ToggleSecurity_WhenSystemInactive_ReturnsFalse()
    {
        var smartHome = new SmartHome();

        bool result = smartHome.ToggleSecurity();

        Assert.False(result);
        Assert.False(smartHome.IsSecurityArmed);
    }

    [Fact]
    public void ToggleSecurity_WhenSystemActive_TogglesSecurity()
    {
        var smartHome = new SmartHome();
        smartHome.ActivateSystem();

        // Первое переключение - включаем
        bool result1 = smartHome.ToggleSecurity();
        Assert.True(result1);
        Assert.True(smartHome.IsSecurityArmed);
        Assert.False(smartHome.AreLightsOn); // Проверяем, что свет выключился

        // Второе переключение - выключаем
        bool result2 = smartHome.ToggleSecurity();
        Assert.True(result2);
        Assert.False(smartHome.IsSecurityArmed);
    }

    [Theory]
    [InlineData(15.0, true)]
    [InlineData(10.0, true)]
    [InlineData(9.9, false)]
    [InlineData(25.0, true)]
    [InlineData(35.0, true)]
    [InlineData(35.1, false)]
    public void AdjustTemperature_WithVariousValues_ReturnsCorrectResult(
        decimal temp, bool expected)
    {
        var smartHome = new SmartHome();
        smartHome.ActivateSystem();

        bool result = smartHome.AdjustTemperature(temp);

        Assert.Equal(expected, result);
        if (expected)
        {
            Assert.Equal(temp, smartHome.CurrentTemperature);

            // Проверяем автоматическое управление светом
            bool shouldLightsBeOn = temp < 18.0m || temp > 25.0m;
            Assert.Equal(shouldLightsBeOn, smartHome.AreLightsOn);
        }
    }

    [Fact]
    public void ToggleLights_WhenSecurityArmed_ReturnsFalse()
    {
        var smartHome = new SmartHome();
        smartHome.ActivateSystem();
        smartHome.ToggleSecurity();

        bool result = smartHome.ToggleLights();

        Assert.False(result);
        Assert.False(smartHome.AreLightsOn);
    }

    [Fact]
    public void ToggleLights_WhenAllowed_TogglesLights()
    {
        var smartHome = new SmartHome();
        smartHome.ActivateSystem();

        // Первое переключение - включаем
        bool result1 = smartHome.ToggleLights();
        Assert.True(result1);
        Assert.True(smartHome.AreLightsOn);

        // Второе переключение - выключаем
        bool result2 = smartHome.ToggleLights();
        Assert.True(result2);
        Assert.False(smartHome.AreLightsOn);
    }

    [Fact]
    public void GetSystemStatus_ReturnsCorrectString()
    {
        var smartHome = new SmartHome();
        smartHome.ActivateSystem();
        smartHome.AdjustTemperature(22.5m);
        smartHome.ToggleLights();

        string status = smartHome.GetSystemStatus();

        Assert.Contains("System: Active", status);
        Assert.Contains("Security: Disarmed", status);
        Assert.Contains("Temp: 22,5", status);
        Assert.Contains("Lights: On", status);
    }
}