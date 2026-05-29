using DeskSchedule.Data;
using DeskSchedule.Models;

namespace DeskSchedule.Services;

/// <summary>
/// 日程服务实现
/// </summary>
public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _repository;

    public ScheduleService(IScheduleRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Schedule>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<Schedule?> GetByIdAsync(int id)
    {
        return _repository.GetByIdAsync(id);
    }

    public Task<IEnumerable<Schedule>> GetByDateAsync(DateTime date)
    {
        return _repository.GetByDateAsync(date);
    }

    public Task<IEnumerable<Schedule>> GetTodayAsync()
    {
        return _repository.GetTodayAsync();
    }

    public Task<IEnumerable<Schedule>> GetUpcomingAsync(DateTime maxTime)
    {
        return _repository.GetUpcomingAsync(maxTime);
    }

    public async Task<Schedule> AddAsync(Schedule schedule)
    {
        schedule.CreatedAt = DateTime.Now;
        return await _repository.AddAsync(schedule);
    }

    public async Task<Schedule> UpdateAsync(Schedule schedule)
    {
        schedule.UpdatedAt = DateTime.Now;
        return await _repository.UpdateAsync(schedule);
    }

    public Task DeleteAsync(int id)
    {
        return _repository.DeleteAsync(id);
    }

    public async Task MarkCompletedAsync(int id)
    {
        var schedule = await _repository.GetByIdAsync(id);
        if (schedule != null)
        {
            schedule.IsCompleted = true;
            schedule.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(schedule);
        }
    }

    public async Task MarkUncompletedAsync(int id)
    {
        var schedule = await _repository.GetByIdAsync(id);
        if (schedule != null)
        {
            schedule.IsCompleted = false;
            schedule.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(schedule);
        }
    }
}
