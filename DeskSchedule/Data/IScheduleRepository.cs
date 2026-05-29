using DeskSchedule.Models;

namespace DeskSchedule.Data;

/// <summary>
/// 日程仓储接口
/// </summary>
public interface IScheduleRepository
{
    Task<IEnumerable<Schedule>> GetAllAsync();
    Task<Schedule?> GetByIdAsync(int id);
    Task<IEnumerable<Schedule>> GetByDateAsync(DateTime date);
    Task<IEnumerable<Schedule>> GetTodayAsync();
    Task<IEnumerable<Schedule>> GetUpcomingAsync(DateTime maxTime);
    Task<IEnumerable<Schedule>> GetPendingRemindersAsync(DateTime before);
    Task<Schedule> AddAsync(Schedule schedule);
    Task<Schedule> UpdateAsync(Schedule schedule);
    Task DeleteAsync(int id);
}
