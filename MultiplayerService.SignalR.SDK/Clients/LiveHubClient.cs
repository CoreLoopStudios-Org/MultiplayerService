using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using MultiplayerService.SignalR.SDK.Interfaces;
using MultiplayerService.SignalR.SDK.Models;

namespace MultiplayerService.SignalR.SDK.Clients
{
    /// <summary>
    /// SignalR client for the LiveHub
    /// </summary>
    public class LiveHubClient : ILiveHubClient
    {
        private readonly HubConnection _connection;
        private readonly string _hubUrl;
        private bool _disposed;

        /// <summary>
        /// Event raised when a user joins the hub
        /// </summary>
        public event EventHandler<UserJoinedEventArgs>? UserJoined;

        /// <summary>
        /// Event raised when the connection state changes
        /// </summary>
        public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

        /// <summary>
        /// Event raised when an error occurs
        /// </summary>
        public event EventHandler<HubErrorEventArgs>? ErrorOccurred;

        /// <summary>
        /// Gets the current connection state
        /// </summary>
        public bool IsConnected => _connection.State == HubConnectionState.Connected;

        /// <summary>
        /// Gets the current connection state as a string
        /// </summary>
        public string ConnectionState => _connection.State.ToString();

        /// <summary>
        /// Creates a new instance of LiveHubClient
        /// </summary>
        /// <param name="baseUrl">The base URL of the SignalR server (e.g., "http://localhost:5000")</param>
        /// <param name="hubPath">The path to the hub (default: "/chathub")</param>
        public LiveHubClient(string baseUrl, string hubPath = "/chathub")
        {
            _hubUrl = $"{baseUrl.TrimEnd('/')}/{hubPath.TrimStart('/')}";
            
            _connection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .WithAutomaticReconnect(new[] 
                {
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                })
                .Build();

            SetupEventHandlers();
        }

        /// <summary>
        /// Creates a new instance of LiveHubClient with a pre-configured HubConnection
        /// </summary>
        /// <param name="connection">The pre-configured HubConnection</param>
        public LiveHubClient(HubConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _hubUrl = connection.ConnectionId ?? string.Empty;
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            // Handle "Joined" event from the server
            _connection.On<string>("Joined", (userName) =>
            {
                UserJoined?.Invoke(this, new UserJoinedEventArgs { UserName = userName });
            });

            // Handle connection state changes
            _connection.Reconnecting += (exception) =>
            {
                var previousState = _connection.State.ToString();
                ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs
                {
                    PreviousState = previousState,
                    NewState = "Reconnecting"
                });

                if (exception != null)
                {
                    ErrorOccurred?.Invoke(this, new HubErrorEventArgs
                    {
                        Exception = exception,
                        Message = $"Connection lost. Reconnecting... {exception.Message}"
                    });
                }

                return Task.CompletedTask;
            };

            _connection.Reconnected += (connectionId) =>
            {
                ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs
                {
                    PreviousState = "Reconnecting",
                    NewState = "Connected"
                });

                return Task.CompletedTask;
            };

            _connection.Closed += (exception) =>
            {
                ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs
                {
                    PreviousState = "Connected",
                    NewState = "Disconnected"
                });

                if (exception != null)
                {
                    ErrorOccurred?.Invoke(this, new HubErrorEventArgs
                    {
                        Exception = exception,
                        Message = $"Connection closed. {exception.Message}"
                    });
                }

                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// Connects to the SignalR hub
        /// </summary>
        public async Task ConnectAsync()
        {
            try
            {
                if (_connection.State == HubConnectionState.Disconnected)
                {
                    await _connection.StartAsync();
                    ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs
                    {
                        PreviousState = "Disconnected",
                        NewState = "Connected"
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, new HubErrorEventArgs
                {
                    Exception = ex,
                    Message = $"Failed to connect: {ex.Message}"
                });
                throw;
            }
        }

        /// <summary>
        /// Disconnects from the SignalR hub
        /// </summary>
        public async Task DisconnectAsync()
        {
            try
            {
                if (_connection.State != HubConnectionState.Disconnected)
                {
                    await _connection.StopAsync();
                    ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs
                    {
                        PreviousState = "Connected",
                        NewState = "Disconnected"
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, new HubErrorEventArgs
                {
                    Exception = ex,
                    Message = $"Failed to disconnect: {ex.Message}"
                });
                throw;
            }
        }

        /// <summary>
        /// Joins the hub with the specified username
        /// </summary>
        /// <param name="userName">The username to join with</param>
        public async Task JoinAsync(string userName)
        {
            try
            {
                await _connection.InvokeAsync("Join", userName);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, new HubErrorEventArgs
                {
                    Exception = ex,
                    Message = $"Failed to join: {ex.Message}"
                });
                throw;
            }
        }

        /// <summary>
        /// Starts the connection and automatically reconnects if disconnected
        /// </summary>
        public async Task StartAsync()
        {
            await ConnectAsync();
        }

        /// <summary>
        /// Disposes the client and closes the connection
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _connection.DisposeAsync().AsTask().Wait();
            }
        }
    }
}
