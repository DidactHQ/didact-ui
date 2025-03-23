using DidactUi.Services;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

#region Setup and bind the appsettings.

var assembly = Assembly.GetExecutingAssembly();
var assemblyName = assembly.GetName().Name;
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
// Support multi-environment appsettings files.
var resourceFileName = string.IsNullOrEmpty(environment)
    ? $"{assemblyName}.appsettings.json"
    : $"{assemblyName}.appsettings.{environment}.json";

// Fetch the appsettings.json file as an embedded resource.
var stream = assembly.GetManifestResourceStream(resourceFileName);
var reader = new StreamReader(stream!);
var json = reader.ReadToEnd();

// Create a new IConfiguration.
var iConfiguration = new ConfigurationBuilder()
    .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
    .Build();

// Bind to the appsettings class.
var appSettings = new AppSettings();
iConfiguration.Bind(appSettings);

#endregion

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Use an embedded file provider for the embedded wwwroot folder.
var embeddedFileProvider = new ManifestEmbeddedFileProvider(assembly, "wwwroot");
app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = embeddedFileProvider
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = embeddedFileProvider,
    RequestPath = string.Empty
});

app.Run();