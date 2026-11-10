using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Zephyr;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(2) };
    private bool _wasRunning;
    private string? _robloxExe;

    public MainWindow()
    {
        InitializeComponent();
        _robloxExe = FindRobloxExecutable();
        UpdateStatus();
        _timer.Tick += (_, _) => PollRoblox();
        _timer.Start();
    }

    private static string? FindRobloxExecutable()
    {
        string versions = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Roblox", "Versions");

        if (!Directory.Exists(versions)) return null;

        try
        {
            return Directory.GetDirectories(versions)
                .Select(d => Path.Combine(d, "RobloxPlayerBeta.exe"))
                .Where(File.Exists)
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .FirstOrDefault();
        }
        catch { return null; }
    }

    private static Process[] GetRobloxProcesses()
    {
        try
        {
            return Process.GetProcessesByName("RobloxPlayerBeta");
        }
        catch { return []; }
    }

    private void UpdateStatus()
    {
        var processes = GetRobloxProcesses();
        bool running = processes.Any(p => !p.HasExited);
        StatusText.Text = running ? "Roblox is running" :
            (_robloxExe is null ? "Roblox not detected" : "Roblox is ready");
        StatusDot.Background = running ? new SolidColorBrush(Color.FromRgb(91, 210, 145)) :
            (_robloxExe is null ? new SolidColorBrush(Color.FromRgb(224, 180, 91)) :
             new SolidColorBrush(Color.FromRgb(155, 107, 255)));
        InstallText.Text = _robloxExe ?? "Install Roblox from the official website to use local detection.";
        foreach (var p in processes) p.Dispose();
    }

    private void PollRoblox()
    {
        var processes = GetRobloxProcesses();
        bool running = processes.Any(p => !p.HasExited);
        UpdateStatus();

        if (running && !_wasRunning && AutoApplyCheck.IsChecked == true)
            ApplyPriority(false);

        _wasRunning = running;
        foreach (var p in processes) p.Dispose();
    }

    private void ApplyPriority_Click(object sender, RoutedEventArgs e) => ApplyPriority(true);

    private void ApplyPriority(bool showMessage)
    {
        var processes = GetRobloxProcesses();
        int applied = 0;
        ProcessPriorityClass priority = PriorityBox.SelectedIndex == 0
            ? ProcessPriorityClass.Normal : ProcessPriorityClass.AboveNormal;

        foreach (var process in processes)
        {
            try
            {
                if (!process.HasExited)
                {
                    process.PriorityClass = priority;
                    applied++;
                }
            }
            catch (Exception ex)
            {
                LogText.Text = $"Could not change priority: {ex.Message}";
            }
            finally { process.Dispose(); }
        }

        string result = applied > 0
            ? $"Applied {priority} priority to {applied} Roblox process(es)."
            : "No running Roblox client was found.";
        LogText.Text = result;
        if (showMessage) MessageBox.Show(result, "Zephyr", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Launch_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo("https://www.roblox.com/home") { UseShellExecute = true });
            LogText.Text = "Opened the official Roblox website.";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not open Roblox: {ex.Message}", "Zephyr",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Folder_Click(object sender, RoutedEventArgs e)
    {
        string folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox");
        if (!Directory.Exists(folder))
        {
            MessageBox.Show("Roblox installation folder was not found.", "Zephyr",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        Process.Start(new ProcessStartInfo("explorer.exe", folder) { UseShellExecute = true });
    }

    private static void OpenSettings(string uri)
    {
        try { Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true }); }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not open Windows settings: {ex.Message}", "Zephyr",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void GameMode_Click(object sender, RoutedEventArgs e) =>
        OpenSettings("ms-settings:gaming-gamemode");

    private void Graphics_Click(object sender, RoutedEventArgs e) =>
        OpenSettings("ms-settings:display-graphics");
}
