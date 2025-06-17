using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AICalendar.Shared.Models
{
    public class CalendarEvent
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<Participant> Participants { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateEventRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<string> Attendees { get; set; } = new();
    }

    public class UpdateEventRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public List<string>? Attendees { get; set; }
    }

    public class Participant
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public ParticipationStatus Status { get; set; }

    }

public enum ParticipationStatus
{
    Pending,
    Accepted,
    Declined
}



public class User
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
