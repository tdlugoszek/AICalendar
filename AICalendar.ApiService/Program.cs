using AICalendar.ApiService.Application.AI;
//using AICalendar.ApiService.Application.Events;
//using AICalendar.ApiService.Application.User;
using AICalendar.ApiService.Infrastructure.Database;
using AICalendar.ApiService.Infrastructure.Extensions;
using AICalendar.ApiService.Services;
using AICalendar.ServiceDefaults;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using AICalendar.Shared.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceDefaults();
//builder.AddSqlServerDbContext<AiCalendarDbContext>("database");

builder.Services.AddDbContext<CalendarDbContext>(options =>
    //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    options.UseSqlite("Data Source=calendar.db"));

builder.Services.AddScoped<IEventService, EventService>();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddProblemDetails();
//builder.Services.AddUsers();
//builder.Services.AddEvents();


await builder.AddAi(builder.Configuration.GetSection("Ai").Get<AiSettings>());
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.MapAiRoutes();
//app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
