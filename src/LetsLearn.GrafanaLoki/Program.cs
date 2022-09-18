using Serilog;
using Serilog.Debugging;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Grafana.Loki;

SelfLog.Enable(Console.Error);

var builder = WebApplication.CreateBuilder(args);
builder.Host
    .ConfigureLogging(loggingBuilder => loggingBuilder.ClearProviders())
    .UseSerilog((context, configuration) =>
    {
        //configuration.ReadFrom.Configuration(context.Configuration);

        configuration
            .WriteTo.Console(new RenderedCompactJsonFormatter())
            .WriteTo.GrafanaLoki("http://loki:3100", new List<LokiLabel>()
            {
                new()
                {
                    Key = "Application",
                    Value = context.HostingEnvironment.ApplicationName
                },
                new()
                {
                    Key = "Environment",
                    Value = context.HostingEnvironment.EnvironmentName
                }
            });
    });

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (context, httpContext) =>
    {
        context.Set("Method", httpContext.Request.Method);
    };
});

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