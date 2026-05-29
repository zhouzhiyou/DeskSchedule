using DeskSchedule.Data;
using DeskSchedule.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DeskSchedule.Tests.Data;

public class ScheduleRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ScheduleRepository _repository;

    public ScheduleRepositoryTests()
    {
        // 使用内存数据库进行测试
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _repository = new ScheduleRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddSchedule()
    {
        // Arrange
        var schedule = new Schedule
        {
            Title = "Test Schedule",
            RemindTime = DateTime.Now.AddHours(1),
            RemindType = RemindType.Popup
        };

        // Act
        var result = await _repository.AddAsync(schedule);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Test Schedule", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnSchedule()
    {
        // Arrange
        var schedule = new Schedule
        {
            Title = "Test Schedule",
            RemindTime = DateTime.Now.AddHours(1)
        };
        await _repository.AddAsync(schedule);

        // Act
        var result = await _repository.GetByIdAsync(schedule.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(schedule.Id, result.Id);
    }

    [Fact]
    public async Task GetTodayAsync_ShouldReturnTodaySchedules()
    {
        // Arrange
        var todaySchedule = new Schedule
        {
            Title = "Today",
            RemindTime = DateTime.Today.AddHours(10)
        };
        var tomorrowSchedule = new Schedule
        {
            Title = "Tomorrow",
            RemindTime = DateTime.Today.AddDays(1).AddHours(10)
        };

        await _repository.AddAsync(todaySchedule);
        await _repository.AddAsync(tomorrowSchedule);

        // Act
        var result = await _repository.GetTodayAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Today", result.First().Title);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSchedule()
    {
        // Arrange
        var schedule = new Schedule
        {
            Title = "To Delete",
            RemindTime = DateTime.Now.AddHours(1)
        };
        await _repository.AddAsync(schedule);

        // Act
        await _repository.DeleteAsync(schedule.Id);

        // Assert
        var result = await _repository.GetByIdAsync(schedule.Id);
        Assert.Null(result);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
