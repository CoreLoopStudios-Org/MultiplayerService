li# MultiplayerService.SignalR.SDK

A .NET Standard 2.1 SignalR client SDK for connecting to the MultiplayerService SignalR hub. This SDK is compatible with Unity (2020.3+) and CLI applications.

## Features

- **.NET Standard 2.1** - Compatible with Unity and .NET Core/5+/6+/7+/8+ applications
- **Automatic Reconnection** - Built-in reconnection with configurable delays
- **Event-driven Architecture** - Easy-to-use events for connection state changes and hub events
- **Builder Pattern** - Fluent API for advanced configuration

## Installation

### Unity

1. Copy the `MultiplayerService.SignalR.SDK.dll` and its dependencies to your Unity project's `Assets/Plugins` folder
2. Required dependencies (included in the build output):
   - `Microsoft.AspNetCore.SignalR.Client.dll`
   - `Microsoft.AspNetCore.SignalR.Client.Core.dll`
   - `Microsoft.AspNetCore.SignalR.Common.dll`
   - `Microsoft.AspNetCore.SignalR.Protocols.Json.dll`
   - `System.Text.Json.dll`
   - Other System.* dependencies as needed

### .NET CLI

```bash
dotnet add package MultiplayerService.SignalR.SDK
```

Or reference the project directly:

```xml
<ProjectReference Include="path/to/MultiplayerService.SignalR.SDK.csproj" />
```

## Usage

### Basic Usage

```csharp
using MultiplayerService.SignalR.SDK;
using MultiplayerService.SignalR.SDK.Interfaces;

// Create a client
ILiveHubClient client = LiveHubClientFactory.Create("http://localhost:5000", "/chathub");

// Subscribe to events
client.UserJoined += (sender, args) => 
{
    Console.WriteLine($"User joined: {args.UserName}");
};

client.ConnectionStateChanged += (sender, args) => 
{
    Console.WriteLine($"Connection: {args.PreviousState} -> {args.NewState}");
};

client.ErrorOccurred += (sender, args) => 
{
    Console.WriteLine($"Error: {args.Message}");
};

// Connect to the hub
await client.ConnectAsync();

// Join with a username
await client.JoinAsync("Player1");

// Check connection state
if (client.IsConnected)
{
    Console.WriteLine("Connected to hub!");
}

// Disconnect when done
await client.DisconnectAsync();

// Dispose when finished
client.Dispose();
```

### Using the Builder Pattern

```csharp
using System;
using MultiplayerService.SignalR.SDK;

var client = LiveHubClientFactory.CreateBuilder()
    .WithBaseUrl("http://localhost:5000")
    .WithHubPath("/chathub")
    .WithReconnectDelays(
        TimeSpan.FromSeconds(0),
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(30)
    )
    .Configure(builder => 
    {
        // Additional configuration if needed
    })
    .Build();

await client.ConnectAsync();
```

### Unity Example

```csharp
using UnityEngine;
using MultiplayerService.SignalR.SDK;
using MultiplayerService.SignalR.SDK.Interfaces;

public class MultiplayerManager : MonoBehaviour
{
    private ILiveHubClient _client;

    async void Start()
    {
        _client = LiveHubClientFactory.Create("http://your-server:5000", "/chathub");
        
        _client.UserJoined += OnUserJoined;
        _client.ConnectionStateChanged += OnConnectionStateChanged;
        _client.ErrorOccurred += OnError;
        
        await _client.ConnectAsync();
        await _client.JoinAsync("Player_" + SystemInfo.deviceName);
    }

    private void OnUserJoined(object sender, UserJoinedEventArgs args)
    {
        Debug.Log($"User joined: {args.UserName}");
    }

    private void OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs args)
    {
        Debug.Log($"Connection: {args.PreviousState} -> {args.NewState}");
    }

    private void OnError(object sender, HubErrorEventArgs args)
    {
        Debug.LogError($"Error: {args.Message}");
    }

    async void OnDestroy()
    {
        if (_client != null)
        {
            await _client.DisconnectAsync();
            _client.Dispose();
        }
    }
}
```

## API Reference

### ILiveHubClient Interface

| Property | Description |
|----------|-------------|
| `IsConnected` | Gets whether the client is currently connected |
| `ConnectionState` | Gets the current connection state as a string |

| Method | Description |
|--------|-------------|
| `ConnectAsync()` | Connects to the SignalR hub |
| `DisconnectAsync()` | Disconnects from the SignalR hub |
| `JoinAsync(string userName)` | Joins the hub with the specified username |
| `StartAsync()` | Starts the connection (same as ConnectAsync) |
| `Dispose()` | Disposes the client and closes the connection |

| Event | Description |
|-------|-------------|
| `UserJoined` | Raised when a user joins the hub |
| `ConnectionStateChanged` | Raised when the connection state changes |
| `ErrorOccurred` | Raised when an error occurs |

### LiveHubClientFactory

| Method | Description |
|--------|-------------|
| `Create(string baseUrl, string hubPath)` | Creates a client with the specified URL and path |
| `Create(HubConnection connection)` | Creates a client from a pre-configured connection |
| `CreateBuilder()` | Creates a builder for advanced configuration |

### Event Args

- **UserJoinedEventArgs**: Contains `UserName` property
- **ConnectionStateChangedEventArgs**: Contains `PreviousState` and `NewState` properties
- **HubErrorEventArgs**: Contains `Exception` and `Message` properties

## Requirements

- .NET Standard 2.1 compatible runtime
- For Unity: Unity 2020.3 or later with .NET Standard 2.1 support
- SignalR server running the LiveHub at the specified endpoint

## License

MIT License
