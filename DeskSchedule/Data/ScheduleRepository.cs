using DeskSchedule.Models;
using Microsoft.EntityFrameworkCore;

namespace DeskSchedule.Data;

/// <summary>
/// 日程仓储实现
/// </summary>
public class ScheduleRepository : IScheduleRepository
{
    private readonly AppDbContext _context;

    public ScheduleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Schedule>> GetAllAsync()
    {
        return await _context.Schedules
            .OrderBy(s => s.RemindTime)
            .ToListAsync();
    }

    public async Task<Schedule?> GetByIdAsync(int id)
    {
        return await _context.Schedules.FindAsync(id);
    }

    public async Task<IEnumerable<Schedule>> GetByDateAsync(DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);
        return await _context.Schedules
            .Where(s => s.RemindTime >= start && s.RemindTime < end)
            .OrderBy(s => s.RemindTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetTodayAsync()
    {
        return await GetByDateAsync(DateTime.Today);
    }

    public async Task<IEnumerable<Schedule>> GetUpcomingAsync(DateTime maxTime)
    {
        var now = DateTime.Now;
        return await _context.Schedules
            .Where(s => !s.IsCompleted && s.RemindTime >= now && s.RemindTime <= maxTime)
            .OrderBy(s => s.RemindTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetPendingRemindersAsync(DateTime before)
    {
        return await _context.Schedules
            .Where(s => !s.IsCompleted && s.RemindTime <= before)
            .OrderBy(s => s.RemindTime)
            .ToListAsync();
    }

    public async Task<Schedule> AddAsync(Schedule schedule)
    {
        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();
        return schedule;
    }

    public async Task<Schedule> UpdateAsync(Schedule schedule)
    {
        _context.Schedules.Update(schedule);
        await _context.SaveChangesAsync();
        return schedule;
    }

    public async Task DeleteAsync(int id)
    {
        var schedule = await _context.Schedules.FindAsync(id);
        if (schedule != null)
        {
            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
        }
    }
}
