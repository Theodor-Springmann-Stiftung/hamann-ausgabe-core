using HaXMLReader;
using HaXMLReader.Interfaces;
using HaWeb.XMLParser;
using HaWeb.XMLTests;
using HaWeb.FileHelpers;
using Microsoft.FeatureManagement;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Primitives;

var builder = WebApplication.CreateBuilder(args);

// Add additional configuration from Git repository
var fileStoragePath = builder.Configuration.GetValue<string>("FileStoragePath");
if (!string.IsNullOrEmpty(fileStoragePath)) {
    var externalSettingsPath = Path.Combine(fileStoragePath, "GIT", "settings.json");
    builder.Configuration.AddJsonFile(externalSettingsPath, optional: true, reloadOnChange: true);
}

// Create initial Data
var tS = new XMLTestService();
var XMLIS = new XMLInteractionService(builder.Configuration, tS);
var hdW = new HaDocumentWrapper(XMLIS, builder.Configuration);
var gitService = new GitService(builder.Configuration, builder.Services.BuildServiceProvider().GetService<ILogger<GitService>>());
var XMLFP = new XMLFileProvider(XMLIS, hdW, gitService, builder.Configuration);

// Add services to the container.
builder.Services.AddControllers().AddXmlSerializerFormatters();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IXMLTestService, XMLTestService>((_) => tS);
builder.Services.AddSingleton<IXMLInteractionService, XMLInteractionService>((_) => XMLIS);
builder.Services.AddSingleton<IHaDocumentWrappper, HaDocumentWrapper>((_) => hdW);
builder.Services.AddSingleton<IGitService, GitService>((_) => gitService);
builder.Services.AddSingleton<IXMLFileProvider, XMLFileProvider>(_ => XMLFP);
builder.Services.AddSingleton<WebSocketMiddleware>();
builder.Services.AddTransient<IReaderService, ReaderService>();
builder.Services.AddFeatureManagement();


var app = builder.Build();

// Reload config on change
// var cM = new ConfigurationMonitor(configpaths.ToArray(), app.Services);
// ChangeToken.OnChange(
//     () => app.Configuration.GetReloadToken(),
//     (state) => cM.InvokeChanged(state),
//     app.Environment
// );

// // Websockets for realtime notification of changes
app.UseWebSockets(new WebSocketOptions {
    KeepAliveInterval = TimeSpan.FromMinutes(30),
});
app.UseMiddleware<WebSocketMiddleware>();

// Production Options
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
}

app.UseAuthorization();

var cacheMaxAgeOneWeek = (60 * 60 * 24 * 7).ToString();
app.UseStaticFiles(new StaticFileOptions {
    OnPrepareResponse = ctx => {
        ctx.Context.Response.Headers.Add("Cache-Control", "public, max-age=" + cacheMaxAgeOneWeek);

        // Ensure correct MIME types
        var path = ctx.File.PhysicalPath;
        if (path?.EndsWith(".css") == true) {
            ctx.Context.Response.ContentType = "text/css";
        } else if (path?.EndsWith(".js") == true) {
            ctx.Context.Response.ContentType = "application/javascript";
        }
    }
});
app.MapControllers();
app.Run();
