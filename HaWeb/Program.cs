using HaXMLReader;
using HaXMLReader.Interfaces;
using HaDocument.Interfaces;
using HaWeb.XMLParser;
using Microsoft.FeatureManagement;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// // To list physical files from a path provided by configuration:
// var physicalProvider = new PhysicalFileProvider(Configuration.GetValue<string>("StoredFilesPath"));
// // To list physical files in the temporary files folder, use:
// //var physicalProvider = new PhysicalFileProvider(Path.GetTempPath());
// services.AddSingleton<IFileProvider>(physicalProvider);

builder.Services.AddSingleton<ILibrary>(HaDocument.Document.Create(new Options()));
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



class Options : IHaDocumentOptions {
    public string HamannXMLFilePath { get; set; } = HaWeb.Settings.General.XMLFILEPATH;
    public string[] AvailableVolumes { get; set; } = HaWeb.Settings.General.AVAILABLEVOLUMES;
    public bool NormalizeWhitespace { get; set; } = HaWeb.Settings.General.NORMALIZEWHITESPACE;
    public (int, int) AvailableYearRange {get; set; } = HaWeb.Settings.General.AVAILABLEYEARRANGE;
}