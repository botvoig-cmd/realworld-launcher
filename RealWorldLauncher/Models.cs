namespace RealWorldLauncher;

public sealed class Profile
{
    public string Name { get; set; } = "Новый твинк";
    public string GameDirectory { get; set; } = "";
    public string JavaPath { get; set; } = "javaw.exe";
    public int RamMb { get; set; } = 2048;
    public string JvmArgs { get; set; } = "-XX:+UseG1GC";
    public string Proxy { get; set; } = "";
    public string RegisterCommand { get; set; } = "";
    public string LoginCommand { get; set; } = "";
    public bool AutoRegister { get; set; }
    public bool AutoLogin { get; set; }
}

public sealed class ServerPreset
{
    public string Name { get; set; } = "RealWorld";
    public string Address { get; set; } = "play.realworld.ru";
    public string? Port { get; set; }
}

public sealed class RunningInstance
{
    public Profile Profile { get; init; } = new();
    public System.Diagnostics.Process Process { get; init; } = null!;
    public string Status { get; set; } = "Запускается";
}