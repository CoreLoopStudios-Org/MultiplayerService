using System;
using System.Threading.Tasks;
using MultiplayerService.SignalR.SDK.Models;

namespace MultiplayerService.SignalR.SDK.Interfaces
{
    /// <summary>
    /// Interface for the LiveHub SignalR client
    /// </summary>
    public interface ILiveHubClient : IDisposable
    {
        /// <summary>
        /// Event raised when a user joins the hub
        /// </summary>
        event EventHandler<UserJoinedEventArgs>? UserJoined;

        /// <summary>
        /// Event raised when the connection state changes
        /// </summary>
        event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

        /// <summary>
        /// Event raised when an error occurs
        /// </summary>
        event EventHandler<HubErrorEventArgs>? ErrorOccurred;

        /// <summary>
        /// Gets the current connection state
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Gets the current connection state as a string
        /// </summary>
        string ConnectionState { get; }

        /// <summary>
        /// Connects to the SignalR hub
        /// </summary>
        /// <returns>A task representing the async operation</returns>
        Task ConnectAsync();

        /// <summary>
        /// Disconnects from the SignalR hub
        /// </summary>
        /// <returns>A task representing the async operation</returns>
        Task DisconnectAsync();

        /// <summary>
        /// Joins the hub with the specified username
        /// </summary>
        /// <param name="userName">The username to join with</param>
        /// <returns>A task representing the async operation</returns>
        Task JoinAsync(string userName);

        /// <summary>
        /// Starts the connection and automatically reconnects if disconnected
        /// </summary>
        /// <returns>A task representing the async operation</returns>
        Task StartAsync();
    }
}
