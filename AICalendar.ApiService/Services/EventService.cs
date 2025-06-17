using Microsoft.EntityFrameworkCore;
using AICalendar.Shared.Models;
using AICalendar.ApiService.Infrastructure.Database;

namespace AICalendar.ApiService.Services;

public class EventService : IEventService
{
    private readonly CalendarDbContext _context;
    private readonly ILogger<EventService> _logger;

    public EventService(CalendarDbContext context, ILogger<EventService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<CalendarEvent>> GetEventsAsync(DateTime? from, DateTime? to)
    {
        var query = _context.Events
            .Include(e => e.Participants)
            .AsQueryable();

        if (from.HasValue)
            query = query.Where(e => e.Start >= from.Value);

        if (to.HasValue)
            query = query.Where(e => e.End <= to.Value);

        return await query
            .OrderBy(e => e.Start)
            .ToListAsync();
    }

    public async Task<CalendarEvent?> GetEventByIdAsync(Guid id)
    {
        return await _context.Events
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<CalendarEvent> CreateEventAsync(CalendarEvent eventModel)
    {
        eventModel.Id = Guid.NewGuid();
        eventModel.CreatedAt = DateTime.UtcNow;
        eventModel.UpdatedAt = DateTime.UtcNow;

        // Ensure participant IDs are set
        foreach (var participant in eventModel.Participants)
        {
            if (participant.Id == Guid.Empty)
                participant.Id = Guid.NewGuid();
        }

        _context.Events.Add(eventModel);
        await _context.SaveChangesAsync();

        return eventModel;
    }

    public async Task<CalendarEvent> UpdateEventAsync(Guid id, CalendarEvent eventModel)
    {
        var existingEvent = await _context.Events
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (existingEvent == null)
            throw new ArgumentException($"Event with ID {id} not found");

        existingEvent.Title = eventModel.Title;
        existingEvent.Description = eventModel.Description;
        existingEvent.Start = eventModel.Start;
        existingEvent.End = eventModel.End;
        existingEvent.UpdatedAt = DateTime.UtcNow;

        // Update participants if provided
        if (eventModel.Participants.Any())
        {
            existingEvent.Participants.Clear();
            existingEvent.Participants.AddRange(eventModel.Participants);
        }

        await _context.SaveChangesAsync();
        return existingEvent;
    }

    public async Task DeleteEventAsync(Guid id)
    {
        var eventToDelete = await _context.Events.FindAsync(id);
        if (eventToDelete == null)
            throw new ArgumentException($"Event with ID {id} not found");

        _context.Events.Remove(eventToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Participant>> GetParticipantsAsync(Guid eventId)
    {
        var eventItem = await _context.Events
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        return eventItem?.Participants ?? new List<Participant>();
    }

    public async Task<Participant> AddParticipantAsync(Guid eventId, string email)
    {
        var eventItem = await _context.Events
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (eventItem == null)
            throw new ArgumentException($"Event with ID {eventId} not found");

        // Check if participant already exists
        if (eventItem.Participants.Any(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Participant already exists");

        var participant = new Participant
        {
            Id = Guid.NewGuid(),
            Email = email,
            Status = ParticipationStatus.Pending
        };

        eventItem.Participants.Add(participant);
        await _context.SaveChangesAsync();

        return participant;
    }

    public async Task UpdateParticipantStatusAsync(Guid eventId, Guid participantId, ParticipationStatus status)
    {
        var eventItem = await _context.Events
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (eventItem == null)
            throw new ArgumentException($"Event with ID {eventId} not found");

        var participant = eventItem.Participants.FirstOrDefault(p => p.Id == participantId);
        if (participant == null)
            throw new ArgumentException($"Participant with ID {participantId} not found");

        participant.Status = status;
        await _context.SaveChangesAsync();
    }

    public async Task RemoveParticipantAsync(Guid eventId, Guid participantId)
    {
        var eventItem = await _context.Events
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (eventItem == null)
            throw new ArgumentException($"Event with ID {eventId} not found");

        var participant = eventItem.Participants.FirstOrDefault(p => p.Id == participantId);
        if (participant == null)
            throw new ArgumentException($"Participant with ID {participantId} not found");

        eventItem.Participants.Remove(participant);
        await _context.SaveChangesAsync();
    }

    //public async Task<List<CalendarEvent>> SearchEventsAsync(string query)
    //{
    //    return await _context.Events
    //        .Include(e => e.Participants)
    //        .Where(e => e.Title.Contains(query) ||
    //                   e.Description.Contains(query))
    //        .OrderBy(e => e.Start)
    //        .ToListAsync();
    //}
}