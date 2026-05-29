using DeskSchedule.Models;

namespace DeskSchedule.Services;

/// <summary>
/// 日程服务接口
/// </summary>
public interface IScheduleService
{
    Task<IEnumerable<Schedule>> GetAllAsync();
    Task<Schedule?> GetByIdAsync(int id);
    Task<IEnumerable<Schedule>> GetByDateAsync(DateTime date);
    Task<IEnumerable<Schedule>> GetTodayAsync();
    Task<IEnumerable<Schedule>> GetUpcomingAsync(DateTime maxTime);
    Task<Schedule> AddAsync(Schedule schedule);
    Task<Schedule> UpdateAsync(Schedule schedule);
    Task DeleteAsync(int id);
    Task MarkCompletedAsync(int id);
    Task MarkUncompletedAsync(int id);
}
