using System;
using System.Threading.Tasks;
using MultiplayerService.SignalR.SDK;
using MultiplayerService.SignalR.SDK.Interfaces;

namespace MultiplayerService.SignalR.CLI
{
    class Program
    {
        private static ILiveHubClient? _client;
        private static string _serverUrl = "http://localhost:5000";
        private static string _hubPath = "/chathub";
        private static string _userName = $"User_{Environment.MachineName}";

        static async Task Main(string[] args)
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║     MultiplayerService.SignalR.CLI - Test Client         ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            // Parse command line arguments
            ParseArguments(args);

            Console.WriteLine($"Server URL: {_serverUrl}");
            Console.WriteLine($"Hub Path: {_hubPath}");
            Console.WriteLine($"Username: {_userName}");
            Console.WriteLine();

            try
            {
                await RunClientAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        static void ParseArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--server":
                    case "-s":
                        if (i + 1 < args.Length)
                        {
                            _serverUrl = args[++i];
                        }
                        break;
                    case "--hub":
                    case "-h":
                        if (i + 1 < args.Length)
                        {
                            _hubPath = args[++i];
                        }
                        break;
                    case "--user":
                    case "-u":
                        if (i + 1 < args.Length)
                        {
                            _userName = args[++i];
                        }
                        break;
                    case "--help":
                        ShowHelp();
                        Environment.Exit(0);
                        break;
                }
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine("Usage: MultiplayerService.SignalR.CLI [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  -s, --server <url>   Server URL (default: http://localhost:5000)");
            Console.WriteLine("  -h, --hub <path>     Hub path (default: /chathub)");
            Console.WriteLine("  -u, --user <name>    Username (default: User_<machine>)");
            Console.WriteLine("  --help               Show this help message");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  MultiplayerService.SignalR.CLI -s http://localhost:8080 -u Player1");
            Console.WriteLine("  MultiplayerService.SignalR.CLI --server https://myserver.com --hub /livehub");
        }

        static async Task RunClientAsync()
        {
            Console.WriteLine("Creating SignalR client...");
            
            _client = LiveHubClientFactory.Create(_serverUrl, _hubPath);

            // Subscribe to events
            _client.UserJoined += (sender, args) =>
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 🟢 User joined: {args.UserName}");
            };

            _client.ConnectionStateChanged += (sender, args) =>
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 🔄 Connection: {args.PreviousState} -> {args.NewState}");
            };

            _client.ErrorOccurred += (sender, args) =>
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ❌ Error: {args.Message}");
                if (args.Exception != null)
                {
                    Console.WriteLine($"   Exception: {args.Exception.GetType().Name}");
                }
            };

            Console.WriteLine("Connecting to server...");
            try
            {
                await _client.ConnectAsync();
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✅ Connected successfully!");
                Console.WriteLine($"   Connection state: {_client.ConnectionState}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ❌ Failed to connect: {ex.Message}");
                Console.WriteLine("Make sure the SignalR server is running.");
                return;
            }

            // Join the hub
            Console.WriteLine($"Joining hub as '{_userName}'...");
            try
            {
                await _client.JoinAsync(_userName);
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✅ Joined successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ❌ Failed to join: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("Connected! Press any key to disconnect and exit.");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine();

            // Wait for user input
            Console.ReadKey(true);

            Console.WriteLine("Disconnecting...");
            await _client.DisconnectAsync();
            _client.Dispose();
            
            Console.WriteLine("Goodbye!");
        }
    }
}
