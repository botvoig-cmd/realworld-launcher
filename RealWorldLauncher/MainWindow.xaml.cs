using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace RealWorldLauncher;

public partial class MainWindow : Window
{
    readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "launcher.json");
    readonly List<Profile> profiles = [];
    readonly List<ServerPreset> servers = [new() { Name = "RealWorld", Address = "play.realworld.ru" }, new() { Name = "Мега гриф", Address = "mega.realworld.ru" }, new() { Name = "Анархия", Address = "anarchy.realworld.ru" }];
    readonly List<RunningInstance> running = [];

    public MainWindow()
    {
        InitializeComponent();
        LoadConfig();
        ServerBox.ItemsSource = servers; ServerBox.SelectedIndex = 0;
        ProfilesList.ItemsSource = profiles; RunningList.ItemsSource = running;
        if (profiles.Count == 0) profiles.Add(new Profile { Name = "Твинк 1" });
        ProfilesList.Items.Refresh(); ProfilesList.SelectedIndex = 0;
    }

    void LoadConfig() { if (!File.Exists(ConfigPath)) return; try { profiles.AddRange(JsonSerializer.Deserialize<List<Profile>>(File.ReadAllText(ConfigPath)) ?? []); } catch { } }
    void SaveConfig() => File.WriteAllText(ConfigPath, JsonSerializer.Serialize(profiles, new JsonSerializerOptions { WriteIndented = true }));
    void Profile_Changed(object s, SelectionChangedEventArgs e) { if (ProfilesList.SelectedItem is Profile p) { NameBox.Text=p.Name; DirBox.Text=p.GameDirectory; JavaBox.Text=p.JavaPath; RamBox.Text=p.RamMb.ToString(); JvmBox.Text=p.JvmArgs; ProxyBox.Text=p.Proxy; RegBox.Text=p.RegisterCommand; LoginBox.Text=p.LoginCommand; AutoRegBox.IsChecked=p.AutoRegister; AutoLoginBox.IsChecked=p.AutoLogin; } }
    void Save_Click(object s, RoutedEventArgs e) { if (ProfilesList.SelectedItem is not Profile p) return; p.Name=NameBox.Text; p.GameDirectory=DirBox.Text; p.JavaPath=JavaBox.Text; if (int.TryParse(RamBox.Text,out var ram)) p.RamMb=ram; p.JvmArgs=JvmBox.Text; p.Proxy=ProxyBox.Text; p.RegisterCommand=RegBox.Text; p.LoginCommand=LoginBox.Text; p.AutoRegister=AutoRegBox.IsChecked==true; p.AutoLogin=AutoLoginBox.IsChecked==true; SaveConfig(); StatusText.Text="Профиль сохранён"; ProfilesList.Items.Refresh(); }
    void AddProfile_Click(object s, RoutedEventArgs e) { profiles.Add(new Profile { Name=$"Твинк {profiles.Count+1}" }); ProfilesList.Items.Refresh(); ProfilesList.SelectedIndex=profiles.Count-1; }
    void Server_Changed(object s, SelectionChangedEventArgs e) { if (ServerBox.SelectedItem is ServerPreset p) AddressBox.Text=p.Address+(string.IsNullOrWhiteSpace(p.Port)?"":":"+p.Port); }
    void AddServer_Click(object s, RoutedEventArgs e) { if (!string.IsNullOrWhiteSpace(AddressBox.Text)) { servers.Add(new ServerPreset { Name=AddressBox.Text, Address=AddressBox.Text }); ServerBox.Items.Refresh(); ServerBox.SelectedIndex=servers.Count-1; } }
    void Launch_Click(object s, RoutedEventArgs e) { Save_Click(s,e); var address=AddressBox.Text.Trim(); foreach (var p in profiles) Launch(p,address); }
    void Launch(Profile p, string address)
    {
        if (!Directory.Exists(p.GameDirectory)) { StatusText.Text=$"Нет папки игры: {p.Name}"; return; }
        var args=$"-Xms512M -Xmx{Math.Max(512,p.RamMb)}M {p.JvmArgs} -jar minecraft.jar --gameDir \"{p.GameDirectory}\" --server {address}";
        var process=Process.Start(new ProcessStartInfo(p.JavaPath,args){WorkingDirectory=p.GameDirectory,UseShellExecute=false,CreateNoWindow=false});
        if (process != null) { running.Add(new RunningInstance { Profile=p, Process=process, Status=$"{p.Name} · PID {process.Id} · {address}" }); RunningList.Items.Refresh(); StatusText.Text=$"Запущен {p.Name}"; }
    }
    void Mods_Click(object s, RoutedEventArgs e) { if (ProfilesList.SelectedItem is Profile p && !string.IsNullOrWhiteSpace(p.GameDirectory)) { Directory.CreateDirectory(Path.Combine(p.GameDirectory,"mods")); Process.Start("explorer.exe",Path.Combine(p.GameDirectory,"mods")); } }
    void Settings_Click(object s, RoutedEventArgs e) => MessageBox.Show("Версия Minecraft: 1.21.4\nДобавляй Fabric/Forge-моды в папку mods выбранного профиля.\nПрокси указывается как host:port или host:port:user:password.", "Настройки");
}