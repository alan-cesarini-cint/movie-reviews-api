using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Movies.Api.Models;

namespace Movies.Api.Repositories;

public class MovieRepository : IMovieRepository
{
    private const string TableName = "Movies";
    private readonly IAmazonDynamoDB _dynamoDb;

    public MovieRepository(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }

    public async Task AddMovieAsync(Movie movie)
    {
        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = movie.Id } },
                { "Title", new AttributeValue { S = movie.Title } },
                { "Year", new AttributeValue { N = movie.Year.ToString() } },
                { "Genre", new AttributeValue { S = movie.Genre } },
                { "Synopsis", new AttributeValue { S = movie.Synopsis } }
            }
        };

        await _dynamoDb.PutItemAsync(request);
    }

    public async Task<Movie?> GetMovieAsync(string movieId)
    {
        var response = await _dynamoDb.GetItemAsync(new GetItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = movieId } }
            }
        });

        if (response.Item == null || response.Item.Count == 0) return null;

        return new Movie
        {
            Id = response.Item["Id"].S,
            Title = response.Item["Title"].S,
            Year = int.Parse(response.Item["Year"].N),
            Genre = response.Item["Genre"].S,
            Synopsis = response.Item["Synopsis"].S
        };
    }

    public async Task<IEnumerable<Movie>> GetMoviesAsync()
    {
        var scanRequest = new ScanRequest
        {
            TableName = TableName
        };

        var scanResponse = await _dynamoDb.ScanAsync(scanRequest);
        scanResponse.Items.ForEach(item => Console.WriteLine(item["Year"]));
        var movies = scanResponse.Items.Select(item => new Movie
        {
            Id = item["Id"].S,
            Title = item["Title"].S,
            Year = int.Parse(item["Year"].N),
            Genre = item["Genre"].S,
            Synopsis = item["Synopsis"].S
        }).ToList();

        return movies;
    }

    public async Task UpdateMovieAsync(string movieId, Movie movie)
    {
        var key = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = movieId } }
        };

        var attributeUpdates = new Dictionary<string, AttributeValueUpdate>();
        var movieAttributes = new List<string> { "Title", "Year", "Genre", "Synopsis" };
        foreach (var attribute in movieAttributes)
        {
            AttributeValue attributeValue = attribute == "Year" ? new AttributeValue { N = movie.Year.ToString() } : new AttributeValue { S = movie.GetType().GetProperty(attribute)?.GetValue(movie)?.ToString() };
            attributeUpdates[attribute] = new AttributeValueUpdate
            {
                Action = AttributeAction.PUT,
                Value = attributeValue
            };
        }

        var request = new UpdateItemRequest
        {
            TableName = TableName,
            Key = key,
            AttributeUpdates = attributeUpdates
        };

        await _dynamoDb.UpdateItemAsync(request);
    }

    public async Task DeleteMovieAsync(string movieId)
    {
        var deleteRequest = new DeleteItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = movieId } }
            }
        };

        await _dynamoDb.DeleteItemAsync(deleteRequest);
    }
    
    public async Task<IEnumerable<Movie>> GetMoviesBySearchQueryAsync(string searchQuery)
    {
        var scanRequest = new ScanRequest
        {
            TableName = TableName,
            FilterExpression = "contains(Title, :searchQuery)",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":searchQuery", new AttributeValue { S = searchQuery } }
            }
        };

        var scanResponse = await _dynamoDb.ScanAsync(scanRequest);
        
        var movies = scanResponse.Items.Select(item => new Movie
        {
            Id = item["Id"].S,
            Title = item["Title"].S,
            Year = int.Parse(item["Year"].N),
            Genre = item["Genre"].S,
            Synopsis = item["Synopsis"].S
        }).ToList();

        return movies;
    }
}