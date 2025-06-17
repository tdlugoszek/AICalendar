using AICalendar.Shared.Models;

namespace AICalendar.ApiService.Services
{
    public class SchedulerService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SchedulerService> _logger;

        public SchedulerService(HttpClient httpClient, ILogger<SchedulerService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<CalendarEvent> ScheduleEventAsync(
            string[] emails,
            DateTime from,
            DateTime to,
            string title)
        {
            var eventModel = new CalendarEvent
            {
                Title = title,
                Start = from,
                End = to,
                Participants = emails.Select(email => new Participant
                {
                    Email = email,
                    Status = ParticipationStatus.Pending
                }).ToList()
            };

            var response = await _httpClient.PostAsJsonAsync("api/v1/events", eventModel);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CalendarEvent>();
        }
    }
}
