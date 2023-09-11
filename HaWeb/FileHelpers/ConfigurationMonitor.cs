using System.Timers;

namespace HaWeb.FileHelpers;

public class ConfigurationMonitor {
    private System.Timers.Timer? _timer;
    private string[] _paths;
    private (string, byte[])[]? _h;
    private IServiceProvider _serviceProvider;

    public ConfigurationMonitor(string[] paths, IServiceProvider services) {
        _paths = paths;
        _h = _getHash(paths);
        _serviceProvider = services;
    }

    private static (string, byte[])[]? _getHash(string[] paths) {
        if (paths == null || !paths.Any()) return null;
        var ret = new List<(string, byte[])>();
        foreach(var c in paths) 
            ret.Add((c, _computeHash(c)));
        return ret.ToArray();
    }

    private bool isEqual((string, byte[])[]? _h1, (string, byte[])[]? _h2) {
        if (_h1 == null && _h2 == null) return true;
        if (_h1 == null && _h2 != null) return false;
        if (_h2 == null && _h1 != null) return false;
        if (_h1!.Count() != _h2!.Count()) return false;
        foreach (var h1 in _h1!) {
            foreach (var h2 in _h2!) {
                if (h1.Item1 == h2.Item1 && !Enumerable.SequenceEqual(h1.Item2, h2.Item2)) return false;
            }
        }
        return true;
    }

    public void InvokeChanged(IHostEnvironment _) {
        var h = _getHash(_paths);
        if (_timer == null && !isEqual(h, _h)) {
            _h = h;
            _timer = new(8000) { AutoReset = false };
            _timer.Enabled = true;
            _timer.Elapsed += _OnChanged;
        }
    }

    private void _OnChanged(Object source, System.Timers.ElapsedEventArgs e) {
        Console.WriteLine("Configuration changed (ConfigurationMonitor Class)");
        using IServiceScope serviceScope = _serviceProvider.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;
        var cP = provider.GetRequiredService<IConfiguration>();
        var hP = provider.GetRequiredService<IHaDocumentWrappper>();
        hP.ParseConfiguration(cP);
        var fP = provider.GetRequiredService<IXMLFileProvider>();
        fP.ParseConfiguration(cP);   
        // _lifetime.StopApplication();
        _timer = null;
    }

    private static byte[] _computeHash(string filePath) {
        var runCount = 1;

        while(runCount < 4) {
            try {
                if (File.Exists(filePath))
                    using (var fs = File.OpenRead(filePath)) {
                        return System.Security.Cryptography.SHA1
                            .Create().ComputeHash(fs);
                    }
                else {
                    throw new FileNotFoundException();
                }
            }
            catch (IOException ex) {
                if (runCount == 3)
                    throw;

                Thread.Sleep(TimeSpan.FromSeconds(Math.Pow(2, runCount)));
                runCount++;
            }
        }
        return new byte[20];
    }
}