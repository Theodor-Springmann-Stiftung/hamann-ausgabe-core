using HaXMLReader;
using HaXMLReader.Interfaces;
using HaDocument.Interfaces;
using HaWeb.XMLParser;
using Microsoft.FeatureManagement;
using System.Runtime.InteropServices;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// // To get files from a path provided by configuration:
string? filepath = null;
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
    filepath = builder.Configuration.GetValue<string>("StoredFilePathWindows");
} 
else {
    filepath = builder.Configuration.GetValue<string>("StoredFilePathLinux");
}

if (filepath == null) {
    throw new Exception("You need to set a specific Filepath, either StoredFilePathWindows or StoredFilePathLinux");
}

var physicalProvider = new PhysicalFileProvider(filepath);


builder.Services.AddSingleton<IFileProvider>(physicalProvider);
builder.Services.AddSingleton<HaWeb.FileHelpers.IHaDocumentWrappper, HaWeb.FileHelpers.HaDocumentWrapper>();
builder.Services.AddTransient<IReaderService, ReaderService>();
builder.Services.AddSingleton<IXMLService, XMLService>();
builder.Services.AddFeatureManagement();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

// app.UseWebOptimizer();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();
app.Run();
