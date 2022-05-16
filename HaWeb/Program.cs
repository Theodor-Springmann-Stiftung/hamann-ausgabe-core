using HaXMLReader;
using HaXMLReader.Interfaces;
using HaDocument.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ILibrary>(x => HaDocument.Document.Create(new Options()));
builder.Services.AddTransient<IReaderService, ReaderService>();

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
app.UseRouting();
app.MapControllers();
app.Run();

class Options : IHaDocumentOptions {
    public string HamannXMLFilePath { get; set; } = @"Hamann.xml";
    public string[] AvailableVolumes { get; set; } = {  };
    public bool NormalizeWhitespace { get; set; } = true;
    public (int, int) AvailableYearRange {get; set; } = (1751, 1788);
}