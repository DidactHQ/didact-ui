using DidactUi.Exceptions;
using DidactUi.Services;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

#region Read appsettings.json as an embedded resource.

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

#endregion

#region Read uisettings.json as a runtime IConfiguration.

// Get the real EXE directory
var exePath = Process.GetCurrentProcess().MainModule?.FileName;
var exeDirectory = Path.GetDirectoryName(exePath)!;

// Define the path to UiSettings.json
var uiSettingsPath = Path.Combine(exeDirectory, "uisettings.json");

// Build configuration
var uiSettingsIConfiguration = new ConfigurationBuilder()
    .SetBasePath(exeDirectory)
    .AddJsonFile(uiSettingsPath, optional: true, reloadOnChange: true)
    .Build();

var uiSettings = new UiSettings();
uiSettingsIConfiguration.Bind(uiSettings);
builder.Services.AddSingleton(uiSettings);

#endregion

#region Add CORS policy for local dev only for the Nuxt dev server.

var developmentCorsName = "DevelopmentCORS";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: developmentCorsName,
        policy =>
        {
            // Get the Nuxt dev server base url.
            var nuxtDevServerUrl = iConfiguration.GetValue<string>("NuxtDevServerUrl");
     
            if (string.IsNullOrEmpty(nuxtDevServerUrl))
                throw new MissingEnvironmentVariableException("The 'NuxtDevServerUrl' environment variable is missing from the embedded appsettings.json file. Please include this environment variable.");
            
            policy.WithOrigins(nuxtDevServerUrl);
            policy.AllowAnyMethod();
            policy.AllowAnyHeader();
        });
});

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
    // Add CORS policy for local dev only for the Nuxt dev server.
    app.UseCors(developmentCorsName);
}

app.UseHttpsRedirection();
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