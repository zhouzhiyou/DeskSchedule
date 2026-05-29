using DeskSchedule.Helpers;
using DeskSchedule.Models;
using Xunit;

namespace DeskSchedule.Tests.Helpers;

public class TemplateHelperTests
{
    [Fact]
    public void ReplaceVariables_ShouldReplaceAllVariables()
    {
        // Arrange
        var schedule = new Schedule
        {
            Title = "Meeting",
            Description = "Team meeting",
            RemindTime = new DateTime(2024, 1, 15, 14, 30, 0)
        };
        var template = "{title} on {date} at {time}: {description}";

        // Act
        var result = TemplateHelper.ReplaceVariables(template, schedule);

        // Assert
        Assert.Equal("Meeting on 2024-01-15 at 14:30: Team meeting", result);
    }

    [Fact]
    public void ReplaceVariables_ShouldHandleNullDescription()
    {
        // Arrange
        var schedule = new Schedule
        {
            Title = "Test",
            Description = null,
            RemindTime = DateTime.Now
        };
        var template = "{title} - {description}";

        // Act
        var result = TemplateHelper.ReplaceVariables(template, schedule);

        // Assert
        Assert.Equal("Test - ", result);
    }

    [Fact]
    public void GetAvailableVariables_ShouldReturnAllVariables()
    {
        // Act
        var variables = TemplateHelper.GetAvailableVariables();

        // Assert
        Assert.Equal(5, variables.Length);
        Assert.Contains("{title} - 日程标题", variables);
    }
}
