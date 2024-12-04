using Movies.Api;
using Movies.Api.Extensions;
using Movies.Api.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddValidators();
builder.Services.AddRepositoryServices(builder.Configuration);
builder.Services.AddAwsServices();
builder.Services.AddJwtAuthentication("YourSuperSecretKeyMustBe32BytesLong");
builder.Services.AddAuthorizationPolicies();

// Register DynamoDbInitializer
builder.Services.AddTransient<DynamoDbInitializer>();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

// Add Serilog as the logging provider
builder.Host.UseSerilog();

var app = builder.Build();

// Log HTTP requests
app.UseSerilogRequestLogging();

// Call DynamoDbInitializer
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DynamoDbInitializer>();
    await initializer.InitializeAsync(); // Initialize tables and insert default users
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Register middleware to handle errors (with Serilog)
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();