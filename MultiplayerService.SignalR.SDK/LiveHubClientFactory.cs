using System;
using Microsoft.AspNetCore.SignalR.Client;
using MultiplayerService.SignalR.SDK.Clients;
using MultiplayerService.SignalR.SDK.Interfaces;

namespace MultiplayerService.SignalR.SDK
{
    /// <summary>
    /// Factory class for creating LiveHub clients
    /// </summary>
    public static class LiveHubClientFactory
    {
        /// <summary>
        /// Creates a new LiveHub client with the specified base URL
        /// </summary>
        /// <param name="baseUrl">The base URL of the SignalR server (e.g., "http://localhost:5000")</param>
        /// <param name="hubPath">The path to the hub (default: "/chathub")</param>
        /// <returns>A new ILiveHubClient instance</returns>
        public static ILiveHubClient Create(string baseUrl, string hubPath = "/chathub")
        {
            return new LiveHubClient(baseUrl, hubPath);
        }

        /// <summary>
        /// Creates a new LiveHub client from a pre-configured HubConnection
        /// </summary>
        /// <param name="connection">The pre-configured HubConnection</param>
        /// <returns>A new ILiveHubClient instance</returns>
        public static ILiveHubClient Create(HubConnection connection)
        {
            return new LiveHubClient(connection);
        }

        /// <summary>
        /// Creates a new LiveHub client builder for advanced configuration
        /// </summary>
        /// <returns>A new LiveHubClientBuilder instance</returns>
        public static LiveHubClientBuilder CreateBuilder()
        {
            return new LiveHubClientBuilder();
        }
    }

    /// <summary>
    /// Builder class for creating LiveHub clients with advanced configuration
    /// </summary>
    public class LiveHubClientBuilder
    {
        private string _baseUrl = string.Empty;
        private string _hubPath = "/chathub";
        private TimeSpan[]? _reconnectDelays;
        private Action<IHubConnectionBuilder>? _configure;

        /// <summary>
        /// Sets the base URL of the SignalR server
        /// </summary>
        /// <param name="baseUrl">The base URL (e.g., "http://localhost:5000")</param>
        /// <returns>This builder instance</returns>
        public LiveHubClientBuilder WithBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
            return this;
        }

        /// <summary>
        /// Sets the hub path
        /// </summary>
        /// <param name="hubPath">The path to the hub (default: "/chathub")</param>
        /// <returns>This builder instance</returns>
        public LiveHubClientBuilder WithHubPath(string hubPath)
        {
            _hubPath = hubPath;
            return this;
        }

        /// <summary>
        /// Sets custom reconnection delays
        /// </summary>
        /// <param name="delays">Array of delays between reconnection attempts</param>
        /// <returns>This builder instance</returns>
        public LiveHubClientBuilder WithReconnectDelays(params TimeSpan[] delays)
        {
            _reconnectDelays = delays;
            return this;
        }

        /// <summary>
        /// Adds custom configuration to the HubConnectionBuilder
        /// </summary>
        /// <param name="configure">Configuration action</param>
        /// <returns>This builder instance</returns>
        public LiveHubClientBuilder Configure(Action<IHubConnectionBuilder> configure)
        {
            _configure = configure;
            return this;
        }

        /// <summary>
        /// Builds and returns the configured LiveHubClient
        /// </summary>
        /// <returns>A new ILiveHubClient instance</returns>
        public ILiveHubClient Build()
        {
            if (string.IsNullOrEmpty(_baseUrl))
            {
                throw new InvalidOperationException("Base URL must be set before building the client.");
            }

            var hubUrl = $"{_baseUrl.TrimEnd('/')}/{_hubPath.TrimStart('/')}";

            var builder = new HubConnectionBuilder()
                .WithUrl(hubUrl);

            if (_reconnectDelays != null)
            {
                builder.WithAutomaticReconnect(_reconnectDelays);
            }
            else
            {
                builder.WithAutomaticReconnect();
            }

            _configure?.Invoke(builder);

            var connection = builder.Build();
            return new LiveHubClient(connection);
        }
    }
}
