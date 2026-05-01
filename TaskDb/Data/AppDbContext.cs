using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using TaskDb.Models;
namespace TaskDb.Data;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<TaskItem> Tasks { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>().HasData(
            new TaskItem {
                Id = 1,
                Title = "Изучить ASP.NET Core",
                Description = "Контроллеры, маршруты, middleware",
                Priority = "High",
                IsCompleted = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new TaskItem {
                Id = 2,
                Title = "Подключить SQLte через EF Core",
                Description = "Миграции, DbContext, Linq-запросы",
                Priority = "High",
                IsCompleted = false,
                CreatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            },
            new TaskItem {
                Id = 3,
                Title = "Написать README",
                Description = "Описать структуру проектва",
                Priority = "Normal",
                IsCompleted = false,
                CreatedAt = new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc)
            }
    );
    }
}