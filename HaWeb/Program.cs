using HaXMLReader;
using HaXMLReader.Interfaces;
using HaDocument.Interfaces;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ILibrary>(x => HaDocument.Document.Create(new Options()));
builder.Services.AddTransient<IReaderService, ReaderService>();
builder.Services.AddFeatureManagement();

// builder.Services.AddWebOptimizer();

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