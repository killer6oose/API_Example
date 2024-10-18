// File: Program.cs
using ApiExperiment.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<JsonUserDataService>();
builder.Services.AddSingleton<JsonServiceDataService>();

// setup TempData provider
builder.Services.AddSession();
builder.Services.AddRazorPages().AddSessionStateTempDataProvider();

// Register UserDataService with DbContext
builder.Services.AddScoped<UserDataService>();

// Add services to the container.
builder.Services.AddRazorPages();

// Register MappingService
builder.Services.AddSingleton<MappingService>();
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
app.UseSwaggerUI();

app.UseRouting();
app.UseSession();

app.MapRazorPages();
// Map API endpoints
app.MapControllers();

app.Run();
