using AICalendar.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace AICalendar.ApiService.Infrastructure.Database
{
    public class CalendarDbContext : DbContext
    {
        public CalendarDbContext(DbContextOptions<CalendarDbContext> options) : base(options) { }

        public DbSet<CalendarEvent> Events { get; set; }
        public DbSet<Participant> Participants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalendarEvent>()
                .HasMany(e => e.Participants)
                .WithOne()
                .HasForeignKey("EventId");

            modelBuilder.Entity<CalendarEvent>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
