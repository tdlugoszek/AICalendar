using AICalendar.Shared.Data;
using AICalendar.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AICalendar.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalendarController : ControllerBase
{
    private readonly CalendarDbContext _context;

    public CalendarController(CalendarDbContext context)
    {
        _context = context;
    }

    [HttpGet("events")]
    public async Task<ActionResult<IEnumerable<CalendarEvent>>> GetEvents()
    {
        return await _context.CalendarEvents.ToListAsync();
    }

    [HttpGet("events/{id}")]
    public async Task<ActionResult<CalendarEvent>> GetEvent(int id)
    {
        var calendarEvent = await _context.CalendarEvents.FindAsync(id);
        return calendarEvent == null ? NotFound() : calendarEvent;
    }

    [HttpPost("events")]
    public async Task<ActionResult<CalendarEvent>> CreateEvent(CalendarEvent calendarEvent)
    {
        calendarEvent.CreatedAt = DateTime.UtcNow;
        calendarEvent.UpdatedAt = DateTime.UtcNow;

        _context.CalendarEvents.Add(calendarEvent);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEvent), new { id = calendarEvent.Id }, calendarEvent);
    }

    [HttpPut("events/{id}")]
    public async Task<IActionResult> UpdateEvent(int id, CalendarEvent calendarEvent)
    {
        if (id != calendarEvent.Id)
        {
            return BadRequest();
        }

        calendarEvent.UpdatedAt = DateTime.UtcNow;
        _context.Entry(calendarEvent).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await EventExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("events/{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var calendarEvent = await _context.CalendarEvents.FindAsync(id);
        if (calendarEvent == null)
        {
            return NotFound();
        }

        _context.CalendarEvents.Remove(calendarEvent);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> EventExists(int id)
    {
        return await _context.CalendarEvents.AnyAsync(e => e.Id == id);
    }
}