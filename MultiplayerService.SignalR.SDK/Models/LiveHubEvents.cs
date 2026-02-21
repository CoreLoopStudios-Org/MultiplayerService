using System;

namespace MultiplayerService.SignalR.SDK.Models
{
    /// <summary>
    /// Event arguments for when a user joins the hub
    /// </summary>
    public class UserJoinedEventArgs : EventArgs
    {
        /// <summary>
        /// The username of the user who joined
        /// </summary>
        public string UserName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Event arguments for connection state changes
    /// </summary>
    public class ConnectionStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The previous connection state
        /// </summary>
        public string PreviousState { get; set; } = string.Empty;

        /// <summary>
        /// The new connection state
        /// </summary>
        public string NewState { get; set; } = string.Empty;
    }

    /// <summary>
    /// Event arguments for when an error occurs
    /// </summary>
    public class HubErrorEventArgs : EventArgs
    {
        /// <summary>
        /// The exception that occurred
        /// </summary>
        public Exception? Exception { get; set; }

        /// <summary>
        /// The error message
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
