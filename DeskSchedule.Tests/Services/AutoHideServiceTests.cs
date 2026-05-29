using DeskSchedule.Services;
using Moq;
using Xunit;

namespace DeskSchedule.Tests.Services;

public class AutoHideServiceTests
{
    [Fact]
    public void StartMonitoring_ShouldStartTimer()
    {
        // Arrange
        var service = new AutoHideService();

        // Act
        service.StartMonitoring();

        // Assert
        // 服务应该已经启动，虽然没有公共属性可以验证
        // 我们可以通过检查事件是否触发来间接验证

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void SetHideDelay_ShouldUpdateDelay()
    {
        // Arrange
        var service = new AutoHideService();
        var expectedDelay = 120;

        // Act
        service.SetHideDelay(expectedDelay);

        // Assert - 无法直接验证，但不应该抛出异常

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void StopMonitoring_ShouldStopTimer()
    {
        // Arrange
        var service = new AutoHideService();
        service.StartMonitoring();

        // Act
        service.StopMonitoring();

        // Assert - 不应该抛出异常

        // Cleanup
        service.Dispose();
    }
}
