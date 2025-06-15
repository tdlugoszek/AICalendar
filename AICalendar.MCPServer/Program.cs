using AICalendar.MCPServer.Services;
using AICalendar.ServiceDefaults;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add OpenAI Client
builder.Services.AddSingleton(provider =>
{
    var apiKey = builder.Configuration["OpenAI:ApiKey"] ??
                 throw new InvalidOperationException("OpenAI API key not configured");
    return new OpenAIClient(apiKey);
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebClient", policy =>
    {
        policy.WithOrigins("https://localhost:5002", "http://localhost:5002")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddScoped<IAIService, AIService>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowWebClient");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();