using Microsoft.AspNetCore.Mvc;
using AICalendar.Shared.Models;
using AICalendar.ApiService.Services;
using System.ComponentModel.DataAnnotations;

namespace AICalendar.ApiService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IEventService eventService, ILogger<EventsController> logger)
    {
        _eventService = eventService;
        _logger = logger;
    }

    /// <summary>
    /// Get all events with optional date filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CalendarEvent>>> GetEvents(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        try
        {
            var events = await _eventService.GetEventsAsync(from, to);
            return Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving events");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get a specific event by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CalendarEvent>> GetEvent(Guid id)
    {
        try
        {
            var eventItem = await _eventService.GetEventByIdAsync(id);
            if (eventItem == null)
                return NotFound($"Event with ID {id} not found");

            return Ok(eventItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving event {EventId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new event
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CalendarEvent>> CreateEvent([FromBody] CreateEventRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                return BadRequest("Title is required");

            //if (!IsValidEventTime(request.Start, request.End))
            //    return BadRequest("Invalid event time range");

            var eventModel = new CalendarEvent
            {
                Title = request.Title,
                Description = request.Description,
                Start = request.Start,
                End = request.End,
                Participants = request.Attendees?.Select(email => new Participant
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    Status = ParticipationStatus.Pending
                }).ToList() ?? new List<Participant>()
            };

            var createdEvent = await _eventService.CreateEventAsync(eventModel);
            return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update an existing event
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CalendarEvent>> UpdateEvent(Guid id, [FromBody] UpdateEventRequest request)
    {
        try
        {
            var existingEvent = await _eventService.GetEventByIdAsync(id);
            if (existingEvent == null)
                return NotFound($"Event with ID {id} not found");

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(request.Title))
                existingEvent.Title = request.Title;

            if (request.Description != null)
                existingEvent.Description = request.Description;

            if (request.Start.HasValue)
                existingEvent.Start = request.Start.Value;

            if (request.End.HasValue)
                existingEvent.End = request.End.Value;

            if (request.Attendees != null)
            {
                existingEvent.Participants = request.Attendees.Select(email => new Participant
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    Status = ParticipationStatus.Pending
                }).ToList();
            }

            if (!IsValidEventTime(existingEvent.Start, existingEvent.End))
                return BadRequest("Invalid event time range");

            var updatedEvent = await _eventService.UpdateEventAsync(id, existingEvent);
            return Ok(updatedEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event {EventId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete an event
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEvent(Guid id)
    {
        try
        {
            var existingEvent = await _eventService.GetEventByIdAsync(id);
            if (existingEvent == null)
                return NotFound($"Event with ID {id} not found");

            await _eventService.DeleteEventAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event {EventId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get participants for an event
    /// </summary>
    [HttpGet("{id}/participants")]
    public async Task<ActionResult<List<Participant>>> GetParticipants(Guid id)
    {
        try
        {
            var participants = await _eventService.GetParticipantsAsync(id);
            return Ok(participants);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving participants for event {EventId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Add a participant to an event
    /// </summary>
    [HttpPost("{id}/participants")]
    public async Task<ActionResult<Participant>> AddParticipant(Guid id, [FromBody] AddParticipantModel model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.Email))
                return BadRequest("Email is required");

            if (!IsValidEmail(model.Email))
                return BadRequest("Invalid email format");

            var participant = await _eventService.AddParticipantAsync(id, model.Email);
            return CreatedAtAction(nameof(GetParticipants), new { id }, participant);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding participant to event {EventId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update participant status
    /// </summary>
    [HttpPut("{id}/participants/{participantId}")]
    public async Task<ActionResult> UpdateParticipantStatus(
        Guid id,
        Guid participantId,
        [FromBody] UpdateParticipantStatusModel model)
    {
        try
        {
            await _eventService.UpdateParticipantStatusAsync(id, participantId, model.Status);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating participant status for event {EventId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Remove a participant from an event
    /// </summary>
    [HttpDelete("{id}/participants/{participantId}")]
    public async Task<ActionResult> RemoveParticipant(Guid id, Guid participantId)
    {
        try
        {
            await _eventService.RemoveParticipantAsync(id, participantId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing participant from event {EventId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get events for a specific date
    /// </summary>
    [HttpGet("by-date/{date}")]
    public async Task<ActionResult<List<CalendarEvent>>> GetEventsByDate(DateTime date)
    {
        try
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1).AddSeconds(-1);

            var events = await _eventService.GetEventsAsync(startOfDay, endOfDay);
            return Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving events for date {Date}", date);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Search events by title or description
    /// </summary>
    //[HttpGet("search")]
    //public async Task<ActionResult<List<CalendarEvent>>> SearchEvents([FromQuery] string query)
    //{
    //    try
    //    {
    //        if (string.IsNullOrWhiteSpace(query))
    //            return BadRequest("Search query cannot be empty");

    //        var events = await _eventService.SearchEventsAsync(query);
    //        return Ok(events);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error searching events with query {Query}", query);
    //        return StatusCode(500, "Internal server error");
    //    }
    //}

    #region Helper Methods

    private static bool IsValidEventTime(DateTime start, DateTime end)
    {
        return start < end && start >= DateTime.UtcNow.AddMinutes(-5); // Allow 5 minutes tolerance
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Simple Models for API Operations

    public class AddParticipantModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class UpdateParticipantStatusModel
    {
        [Required]
        public ParticipationStatus Status { get; set; }
    }

    #endregion
}