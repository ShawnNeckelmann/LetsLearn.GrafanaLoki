using Serilog;
using Serilog.Debugging;

SelfLog.Enable(Console.Error);

var builder = WebApplication.CreateBuilder(args);
builder.Host
    .ConfigureLogging(loggingBuilder => loggingBuilder.ClearProviders())
    .UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();