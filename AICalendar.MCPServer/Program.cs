using AICalendar.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceDefaults();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowApiClient", configurePolicy: policy =>
    {
        policy.AllowAnyOrigin()
              .WithOrigins("https://localhost:5002") // ApiService URL
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();



var app = builder.Build();
app.MapDefaultEndpoints();
app.UseCors("AllowApiClient");
app.MapMcp();

await app.RunAsync();