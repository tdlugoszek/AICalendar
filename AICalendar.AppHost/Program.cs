using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<AICalendar_ApiService>("api-service")
    .WithHttpEndpoint(port: 5001, name: "api");







builder.Build().Run();
