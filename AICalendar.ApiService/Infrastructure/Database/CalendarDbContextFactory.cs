using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AICalendar.ApiService.Infrastructure.Database
{
    public class CalendarDbContextFactory : IDesignTimeDbContextFactory<CalendarDbContext>
    {
        public CalendarDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CalendarDbContext>();
            optionsBuilder.UseSqlite("Data Source=calendar.db");

            return new CalendarDbContext(optionsBuilder.Options);
        }
    }
}
