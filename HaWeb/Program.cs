using HaXMLReader;
using HaXMLReader.Interfaces;
using HaDocument.Interfaces;
using HaWeb.XMLParser;
using HaWeb.XMLTests;
using HaWeb.FileHelpers;
using HaWeb.BackgroundTask;
using Microsoft.FeatureManagement;
using System.Runtime.InteropServices;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

var builder = WebApplication.CreateBuilder(args);
List<string> configpaths = new List<string>();

// Add additional configuration
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
    var p = builder.Configuration.GetValue<string>("WorkingTreePathWindows") + "settings.json";
    configpaths.Add(p);
    builder.Configuration.AddJsonFile(p, optional: true, reloadOnChange: true);
} else {
    var p = builder.Configuration.GetValue<string>("WorkingTreePathLinux") + "settings.json";
    configpaths.Add(p);
    builder.Configuration.AddJsonFile(p, optional: true, reloadOnChange: true);
}

// Create initial Data
var tS = new XMLTestService(); 
var XMLIS = new XMLInteractionService(builder.Configuration, tS);
var hdW = new HaDocumentWrapper(XMLIS, builder.Configuration);
var XMLFP = new XMLFileProvider(XMLIS, hdW, builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IXMLTestService, XMLTestService>((_) => tS);
builder.Services.AddSingleton<IXMLInteractionService, XMLInteractionService>((_) => XMLIS);
builder.Services.AddSingleton<IHaDocumentWrappper, HaDocumentWrapper>((_) => hdW);
builder.Services.AddSingleton<IXMLFileProvider, XMLFileProvider>(_ => XMLFP);
builder.Services.AddSingleton<WebSocketMiddleware>();
builder.Services.AddTransient<IReaderService, ReaderService>();

// builder.Services.AddSingleton<IConfigurationMonitor, ConfigurationMonitor>();
// builder.Services.AddHostedService<QueuedHostedService>();
// builder.Services.AddSingleton<IBackgroundTaskQueue>(ctx =>
// {
//     if (!int.TryParse(builder.Configuration["QueueCapacity"], out var queueCapacity))
//         queueCapacity = 100;
//     return new BackgroundTaskQueue(queueCapacity);
// });
// builder.Services.AddSingleton<IMonitorLoop, MonitorLoop>();
builder.Services.AddFeatureManagement();
var app = builder.Build();

// Reload config on change
var cM = new ConfigurationMonitor(configpaths.ToArray(), app.Services);
ChangeToken.OnChange(
    () => app.Configuration.GetReloadToken(),
    (state) => cM.InvokeChanged(state),
    app.Environment
);

app.UseWebSockets( new WebSocketOptions {
    KeepAliveInterval = TimeSpan.FromMinutes(180),
});
app.UseMiddleware<WebSocketMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
    app.UseForwardedHeaders(new ForwardedHeadersOptions{ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto});
}

app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();
app.Run();
