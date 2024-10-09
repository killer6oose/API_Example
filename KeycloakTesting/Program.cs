using KeycloakTesting.Data;
using KeycloakTesting.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<JsonUserDataService>();

// Register UserDataService with DbContext
builder.Services.AddScoped<UserDataService>();



// Add services to the container.
builder.Services.AddRazorPages();

// Register MappingService
builder.Services.AddSingleton<MappingService>();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

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

//not using an SSL cert, will uncomment if added
//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

// Map API endpoints
app.MapControllers();

//removed the below for testing
//app.MapGet("/api/userdata", ([FromServices] JsonUserDataService userDataService) =>
//{
//    return Results.Ok(userDataService.GetAllUserData());
//});

app.Run();
