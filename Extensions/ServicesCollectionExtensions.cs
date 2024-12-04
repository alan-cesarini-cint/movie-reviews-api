using System.Text;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime.CredentialManagement;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Movies.Api.Repositories;
using Movies.Api.Validators;

namespace Movies.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register repositories
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IMovieRepository, MovieRepository>();
        services.AddTransient<IReviewRepository, ReviewRepository>();

        // Register DynamoDbInitializer
        services.AddTransient<DynamoDbInitializer>();

        return services;
    }

    public static IServiceCollection AddAwsServices(this IServiceCollection services, string profileName = "personal")
    {
        var sharedFile = new SharedCredentialsFile();
        if (sharedFile.TryGetProfile(profileName, out var profile))
        {
            var credentials = profile.GetAWSCredentials(sharedFile);
            var dbConfig = new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(profile.Region.SystemName)
            };
            services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(credentials, dbConfig));
        }
        else
        {
            throw new Exception($"AWS profile '{profileName}' not found.");
        }

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddControllers()
            .AddFluentValidation(config =>
            {
                config.RegisterValidatorsFromAssemblyContaining<MovieValidator>();
                config.RegisterValidatorsFromAssemblyContaining<ReviewValidator>();
            });

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, string key)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

        return services;
    }

    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
            options.AddPolicy("UserPolicy", policy => policy.RequireRole("User", "Admin"));
        });

        return services;
    }
}