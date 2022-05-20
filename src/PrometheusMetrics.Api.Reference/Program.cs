using Prometheus;

// This does the conversion of dotnet event counters + net6 meter metrics into prometheus format
var eventCounterRegistration = EventCounterAdapter.StartListening();
var metricAdapterRegistration = MeterAdapter.StartListening();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// This will create metrics for each HTTP endpoint, giving us 'number of requests', 'request times' etc 
app.UseHttpMetrics(options =>
{
    options.ReduceStatusCodeCardinality();
    options.AddCustomLabel("host", context => context.Request.Host.Host);
    options.AddCustomLabel("logicalService", context => "my-service-name");
});

app.UseHttpsRedirection();

app.MapControllers();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    // This exposes the /metrics endpoint exporter, which will then be used for scraping by a prometheus 
    // instance deployed on the cluster. 
    endpoints.MapMetrics();
});

app.Run();

// Tidy up when app stops running
eventCounterRegistration.Dispose();
metricAdapterRegistration.Dispose();
