using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Movies.Api.Models;

namespace Movies.Api.Repositories;

public class UserRepository : IUserRepository
{
    private const string TableName = "Users";
    private readonly IAmazonDynamoDB _dynamoDb;

    public UserRepository(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }

    public async Task AddUserAsync(User user)
    {
        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = user.Id } },
                { "Email", new AttributeValue { S = user.Email } },
                { "Password", new AttributeValue { S = user.Password } },
                { "Name", new AttributeValue { S = user.Name } },
                { "Role", new AttributeValue { S = user.Role } }
            }
        };

        await _dynamoDb.PutItemAsync(request);
    }

    public async Task<User?> GetUserAsync(string userId)
    {
        var response = await _dynamoDb.GetItemAsync(new GetItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = userId } }
            }
        });

        if (response.Item == null || response.Item.Count == 0) return null;

        return new User
        {
            Id = response.Item["Id"].S,
            Email = response.Item["Email"].S,
            Password = response.Item["Password"].S,
            Name = response.Item["Name"].S,
            Role = response.Item["Role"].S
        };
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var response = await _dynamoDb.ScanAsync(new ScanRequest
        {
            TableName = TableName,
            FilterExpression = "Email = :email",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":email", new AttributeValue { S = email } }
            }
        });

        var item = response.Items.FirstOrDefault();
        if (item == null) return null;

        return new User
        {
            Id = item["Id"].S,
            Email = item["Email"].S,
            Password = item["Password"].S,
            Name = item["Name"].S,
            Role = item["Role"].S
        };
    }
}