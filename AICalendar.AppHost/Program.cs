var builder = DistributedApplication.CreateBuilder(args);

//var sql = builder.AddSqlServer("sql", "sa", "YourPassword123!")
//    .AddDatabase("AICalendar");


var apiService = builder.AddProject<Projects.AICalendar_ApiService>("apiservice");
  // .WithReference(sql);


var mcpServer = builder.AddProject<Projects.AICalendar_MCPServer>("mcp-server");


builder.AddProject<Projects.AICalendar_Web>("web")
    .WithReference(apiService);

builder.Build().Run();