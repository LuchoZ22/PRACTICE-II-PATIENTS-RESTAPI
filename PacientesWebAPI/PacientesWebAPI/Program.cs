using Microsoft.OpenApi.Models;
using PacientesWebAPI.Middleware;
using Serilog;
using UPB.BussinessLogic.Managers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<PatientManager>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = builder.Environment.EnvironmentName,
        
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName.Equals("QA"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.DocumentTitle = app.Environment.EnvironmentName);
    Log.Logger = new LoggerConfiguration()
        .WriteTo.File(builder.Configuration.GetSection("Logging").GetSection("FileLocation").Value + "logs-.log", rollingInterval: RollingInterval.Day)
        .CreateLogger();
    
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.DocumentTitle = app.Environment.EnvironmentName);

    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File(builder.Configuration.GetSection("Logging").GetSection("FileLocation").Value + "logs-.log", rollingInterval: RollingInterval.Day)
        .CreateLogger();
    
}


Log.Information("Initializing the server");

app.UseExceptionHandlerMiddleware();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
