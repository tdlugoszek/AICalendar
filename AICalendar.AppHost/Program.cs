using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AICalendar_ApiService>("api-service")
    .WithHttpEndpoint(port: 5001, name: "api");

var appService = builder.AddProject<Projects.AICalendar_AppService>("app-service")
    .WithHttpEndpoint(port: 5003, name: "app");

var mcpServer = builder.AddProject<Projects.AICalendar_MCPServer>("mcp-server")
    .WithHttpEndpoint(port: 5004, name: "mcp");

var web = builder.AddProject<Projects.AICalendar_Web>("web")
    .WithHttpEndpoint(port: 5002, name: "web")
    .WithReference(apiService)
    .WithReference(mcpServer);

var webApp = builder.AddProject<Projects.AICalendar_WebApp>("webapp")
    .WithHttpEndpoint(port: 5005, name: "webapp")
    .WithReference(apiService);




builder.Build().Run();
