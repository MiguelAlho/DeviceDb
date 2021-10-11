using DeviceDb.Api.Adaptors;
using DeviceDb.Api.Adaptors.Sql.Migrations;
using DeviceDb.Api.Domain.Devices;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["Database"] ?? throw new InvalidOperationException("Database connection string is not set");

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "DeviceDb", 
        Description = "Minimal REST API for Device management", 
        Version = "v1" });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
builder.Services.AddControllers();
builder.Services.AddMvc().AddNewtonsoftJson();      //for json patch support
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.Conventions.Add(new VersionByNamespaceConvention());
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSQLite()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(typeof(Program).Assembly).For.Migrations()
    )
    .AddLogging(lb => lb.AddFluentMigratorConsole())
    .BuildServiceProvider(false);

builder.Services.AddSingleton<IDeviceRepository>(new SqlDeviceRepository(connectionString));



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
app.MapFallback(() => Results.Redirect("/swagger"));

//Database
app.Migrate();

app.Run();


public partial class Program
{
    // Expose the Program class for use with WebApplicationFactory<T>
}