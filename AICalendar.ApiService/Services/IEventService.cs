using AICalendar.Shared.Models;

namespace AICalendar.ApiService.Services
{
    public interface IEventService
    {
        Task<List<CalendarEvent>> GetEventsAsync(DateTime? from, DateTime? to);
        Task<CalendarEvent> GetEventByIdAsync(Guid id);
        Task<CalendarEvent> CreateEventAsync(CalendarEvent eventModel);
        Task<CalendarEvent> UpdateEventAsync(Guid id, CalendarEvent eventModel);
        Task DeleteEventAsync(Guid id);
        Task<List<Participant>> GetParticipantsAsync(Guid eventId);
        Task<Participant> AddParticipantAsync(Guid eventId, string email);
        Task UpdateParticipantStatusAsync(Guid eventId, Guid participantId, ParticipationStatus status);
        Task RemoveParticipantAsync(Guid eventId, Guid participantId);
    }
}
