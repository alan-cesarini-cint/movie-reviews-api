using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Movies.Api.Models;

namespace Movies.Api.Repositories;

public class ReviewRepository : IReviewRepository
{
    private const string TableName = "Reviews";
    private readonly IAmazonDynamoDB _dynamoDb;

    public ReviewRepository(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }

    public async Task AddReviewAsync(Review review)
    {
        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = review.Id } },
                { "MovieId", new AttributeValue { S = review.MovieId } },
                { "UserId", new AttributeValue { S = review.UserId } },
                { "Rating", new AttributeValue { N = review.Rating.ToString() } },
                { "Comment", new AttributeValue { S = review.Comment } }
            }
        };

        await _dynamoDb.PutItemAsync(request);
    }

    public async Task<Review?> GetReviewAsync(string reviewId)
    {
        var response = await _dynamoDb.GetItemAsync(new GetItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = reviewId } }
            }
        });

        if (response.Item == null || response.Item.Count == 0) return null;

        return new Review
        {
            Id = response.Item["Id"].S,
            MovieId = response.Item["MovieId"].S,
            UserId = response.Item["UserId"].S,
            Rating = int.Parse(response.Item["Rating"].N),
            Comment = response.Item["Comment"].S
        };
    }

    public async Task<IEnumerable<Review>> GetReviewsAsync()
    {
        var scanRequest = new ScanRequest
        {
            TableName = TableName
        };

        var scanResponse = await _dynamoDb.ScanAsync(scanRequest);
        var reviews = scanResponse.Items.Select(item => new Review
        {
            Rating = int.Parse(item["Rating"].N),
            Comment = item["Comment"].S
        }).ToList();

        return reviews;
    }
    
    public async Task<IEnumerable<Review>> GetReviewsByMovieIdAsync(string movieId)
    {
        var scanRequest = new ScanRequest
        {
            TableName = TableName,
            FilterExpression = "MovieId = :movieId",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":movieId", new AttributeValue { S = movieId } }
            }
        };

        var scanResponse = await _dynamoDb.ScanAsync(scanRequest);
        var reviews = scanResponse.Items.Select(item => new Review
        {
            UserId = item["UserId"].S,
            MovieId = item["MovieId"].S,
            Rating = int.Parse(item["Rating"].N),
            Comment = item["Comment"].S
        }).ToList();

        return reviews;
    }

    public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(string userId)
    {
        var scanRequest = new ScanRequest
        {
            TableName = TableName,
            FilterExpression = "UserId = :userId",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":userId", new AttributeValue { S = userId } }
            }
        };

        var scanResponse = await _dynamoDb.ScanAsync(scanRequest);
        var reviews = scanResponse.Items.Select(item => new Review
        {
            UserId = item["UserId"].S,
            MovieId = item["MovieId"].S,
            Rating = int.Parse(item["Rating"].N),
            Comment = item["Comment"].S
        }).ToList();

        return reviews;
    }
}