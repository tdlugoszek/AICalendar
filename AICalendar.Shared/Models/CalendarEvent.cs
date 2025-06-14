namespace AICalendar.Shared.Models;

public class CalendarEvent
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public List<string> Attendees { get; set; } = new List<string>();
    public EventPriority Priority { get; set; }
    public bool IsAllDay { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}

public enum EventPriority
{
    Low,
    Normal,
    High,
    Critical
}
