using Microsoft.Extensions.Hosting;

namespace PlayerService.Services;

public class ConsoleInterfaceService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = Task.Run(() => RunConsoleLoop(stoppingToken), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private static async void RunConsoleLoop(CancellationToken stoppingToken)
    {
        await Task.Delay(1000, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            ShowMenu();
            var input = Console.ReadLine();

            if (input == "3" || input?.ToLower() == "exit")
                break;
        }
    }

    private static void ShowMenu()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;

        WriteCentered("╔══════════════════════════════════════════════╗");
        WriteCentered("║                                              ║");
        WriteCentered("║           MATCHER // GAME TERMINAL           ║");
        WriteCentered("║                                              ║");
        WriteCentered("╚══════════════════════════════════════════════╝");
        Console.ResetColor();

        Console.WriteLine();
        TypeOut("Connecting to matchmaking server", ConsoleColor.DarkGray);
        DotsAnimation(10);
        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Green;
        WriteCentered("======================== MATCHMAKING CLIENT ========================");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine("  [1]  🎮  Join Game");
        Console.WriteLine("  [2]  📡  Check Status");
        Console.WriteLine("  [3]  🚪  Exit");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Select option ▶ ");
        Console.ResetColor();
    }

    private static void TypeOut(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        foreach (var c in text)
        {
            Console.Write(c);
            Thread.Sleep(30);
        }
        Console.ResetColor();
    }

    private static void DotsAnimation(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Thread.Sleep(400);
            Console.Write(".");
        }
        Console.WriteLine();
    }

    private static void WriteCentered(string text)
    {
        int width = Console.WindowWidth;
        int left = Math.Max(0, (width - text.Length) / 2);

        Console.SetCursorPosition(left, Console.CursorTop);
        Console.WriteLine(text);
    }
}
