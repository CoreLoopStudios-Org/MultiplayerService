using SignalRDemo.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<LiveHub>("/chathub");

app.Run();