using DeskSchedule.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace DeskSchedule.Data;

/// <summary>
/// 数据库上下文
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<Schedule> Schedules => Set<Schedule>();

    private readonly string? _dbPath;

    public AppDbContext()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataPath, "DeskSchedule");
        Directory.CreateDirectory(appFolder);
        _dbPath = Path.Combine(appFolder, "deskschedule.db");
    }

    public AppDbContext(string dbPath)
    {
        _dbPath = dbPath;
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        _dbPath = null;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured && !string.IsNullOrEmpty(_dbPath))
        {
            options.UseSqlite($"Data Source={_dbPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.ToTable("schedules");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.RemindType).HasConversion<int>();
            entity.Property(e => e.Priority).HasConversion<int>().HasDefaultValue(Priority.Normal);
            entity.HasIndex(e => e.RemindTime);
            entity.HasIndex(e => e.IsCompleted);
        });
    }
}
