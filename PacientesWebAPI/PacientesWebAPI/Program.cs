using Serilog;
using UPB.BussinessLogic.Managers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<PatientManager>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
    Console.WriteLine("IN DEVELOPMENT");
}

if (app.Environment.Equals("QA"))
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.File(builder.Configuration.GetSection("Logging").GetSection("FileLocation").Value + "logs-.log", rollingInterval: RollingInterval.Day)
        .CreateLogger();
    Console.WriteLine("EN QA");
}


Log.Information("Initializing the server");


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
