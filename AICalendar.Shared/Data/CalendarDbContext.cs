using AICalendar.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace AICalendar.Shared.Data;

public class CalendarDbContext : DbContext
{
    public CalendarDbContext(DbContextOptions<CalendarDbContext> options) : base(options)
    {
    }

    public DbSet<CalendarEvent> CalendarEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CalendarEvent>()
            .Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        // Convert List<string> Attendees to comma-separated string for storage
        modelBuilder.Entity<CalendarEvent>()
            .Property(e => e.Attendees)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            );
    }
}