using AICalendar.Shared.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace AICalendar.MCPServer;

[McpServerToolType, Description("Schedules an event to users with emails on specific date and time with title")]
public static class Scheduler
{
    public static async Task<CalendarEvent> ScheduleEvent(
        HttpClient httpClient,
        string[] emails,
        DateTime from,
        DateTime to,
        string title)
    {
        var result = await httpClient.PostAsJsonAsync("https://localhost:5002/api/v1/events", new
        {
            Title = title,
            Start = from,
            End = to,
            Participants = emails.Select(email => new Participant
            {
                Email = email,
                Status = ParticipationStatus.Pending
            }).ToList()
        });
        return await result.Content.ReadFromJsonAsync<CalendarEvent>();
    }

    [McpServerTool, Description("Get list of events on specific time range")]
    public static async Task<List<CalendarEvent>> GetEvents(
        HttpClient httpClient,
        DateTime from,
        DateTime to)
    {
        var events = await httpClient.GetFromJsonAsync<List<CalendarEvent>>(
            $"https://localhost:5002/api/v1/events?from={from:O}&to={to:O}");
        return events ?? [];
    }

    [McpServerTool, Description("Cancel event by id")]
    public static async Task CancelEvent(HttpClient httpClient, Guid id)
    {
        await httpClient.DeleteAsync($"https://localhost:5002/api/v1/events/{id}");
    }
}