// File: Program.cs
using ApiExperiment.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<JsonUserDataService>();
builder.Services.AddSingleton<JsonServiceDataService>();
builder.Services.AddSingleton<ApiExperiment.Helpers.GraphHelper>();
builder.Services.AddSingleton<ApiExperiment.Helpers.MailHelper>();

// setup TempData provider
builder.Services.AddSession();
builder.Services.AddRazorPages().AddSessionStateTempDataProvider();

// Register UserDataService with DbContext
builder.Services.AddScoped<UserDataService>();

// Add services to the container.
builder.Services.AddRazorPages();

// Register MappingService
builder.Services.AddControllers();
//detailed logging
builder.Logging.AddConsole();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Experiment",
        Version = "v1",
        Description = "An API for experimenting with user and service data.",
        Contact = new OpenApiContact
        {
            Name = "CronoTech Consulting LLC",
            Email = "support@cronotech.us",
            Url = new Uri("https://www.cronotech.us")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML comments
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Not using an SSL certificate; uncomment if added
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Experiment v1");
    c.DocumentTitle = "API Experiment API Documentation";
    c.IndexStream = () => File.OpenRead("wwwroot/swagger/index.html");
});

app.UseRouting();
app.UseSession();

app.MapRazorPages();
// Map API endpoints
app.MapControllers();

app.Run();
