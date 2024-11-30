using System.Text;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Movies.Api;
using Movies.Api.Middleware;
using Movies.Api.Repositories;
using Movies.Api.Utils;
using Movies.Api.Validators;
using Newtonsoft.Json;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddAWSService<IAmazonDynamoDB>();

// Register the movie validator
builder.Services.AddControllers()
    .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<MovieValidator>());

// Register the review validator
builder.Services.AddControllers()
    .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<ReviewValidator>());

var credentialsFilePath = "aws_credentials.json";
var json = File.ReadAllText(credentialsFilePath);
var credentials = JsonConvert.DeserializeObject<AwsCredentials>(json);

builder.Services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(
    new BasicAWSCredentials(credentials.AwsAccessKeyId, credentials.AwsSecretAccessKey), // Add your AWS credentials
    new AmazonDynamoDBConfig
    {
        ServiceURL =
            "https://dynamodb." + credentials.Region +
            ".amazonaws.com" // Optional for explicitly pointing to the service URL
    }
));

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IMovieRepository, MovieRepository>();
builder.Services.AddTransient<IReviewRepository, ReviewRepository>();

// Register DynamoDbInitializer
builder.Services.AddTransient<DynamoDbInitializer>();

// JWT configuration - provided by ChatGPT
var key = "YourSuperSecretKeyMustBe32BytesLong"; // Replace with a secure key
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yourapp.com",
            ValidAudience = "yourapp.com",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User", "Admin"));
});

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