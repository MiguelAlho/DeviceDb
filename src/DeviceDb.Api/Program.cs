using DeviceDb.Api.Adaptors;
using DeviceDb.Api.Domain.Devices;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DeviceDb", Description = "Minimal REST API for Device management", Version = "v1" });
});
builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.Conventions.Add(new VersionByNamespaceConvention());
});

builder.Services.AddSingleton<IDeviceRepository>(new InMemoryDeviceRepository());


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DeviceDb V1");
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();    
}    
app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();


public partial class Program
{
    // Expose the Program class for use with WebApplicationFactory<T>
}