using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Movies.Api.Utils;

namespace Movies.Api;

// Class provided by ChatGPT
public class DynamoDbInitializer
{
    private readonly IAmazonDynamoDB _dynamoDb;

    public DynamoDbInitializer(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
    }

    public async Task InitializeAsync()
    {
        var usersTableCreated = await EnsureTableExistsAsync("Users");
        await EnsureTableExistsAsync("Movies");
        await EnsureTableExistsAsync("Reviews");

        if (usersTableCreated)
        {
            await AddDefaultUsersAsync();
        }
    }

    private async Task<bool> EnsureTableExistsAsync(string tableName)
    {
        var existingTables = await _dynamoDb.ListTablesAsync();

        if (!existingTables.TableNames.Contains(tableName))
        {
            Console.WriteLine($"Creating table: {tableName}");
            await _dynamoDb.CreateTableAsync(new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition { AttributeName = "Id", AttributeType = "S" }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement { AttributeName = "Id", KeyType = "HASH" } // Partition Key
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                }
            });

            // Wait until the table is active
            await WaitForTableToBecomeActiveAsync(tableName);

            return true; // Indicate that the table was created
        }

        return false; // Table already exists
    }

    private async Task WaitForTableToBecomeActiveAsync(string tableName)
    {
        Console.WriteLine($"Waiting for table {tableName} to become active...");
        while (true)
        {
            var tableStatus = await _dynamoDb.DescribeTableAsync(new DescribeTableRequest
            {
                TableName = tableName
            });

            if (tableStatus.Table.TableStatus == TableStatus.ACTIVE)
            {
                Console.WriteLine($"Table {tableName} is active.");
                break;
            }

            await Task.Delay(1000); // Wait 1 second before checking again
        }
    }

    private async Task AddDefaultUsersAsync()
    {
        var tableName = "Users";

        // User data
        var users = new List<Dictionary<string, AttributeValue>>
        {
            new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = Guid.NewGuid().ToString() } },
                { "Name", new AttributeValue { S = "Regular User" } },
                { "Email", new AttributeValue { S = "user@example.com" } },
                { "Password", new AttributeValue { S = PasswordHasher.HashPassword("password1") } },
                { "Role", new AttributeValue { S = "User" } }
            },
            new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = Guid.NewGuid().ToString() } },
                { "Name", new AttributeValue { S = "Admin" } },
                { "Email", new AttributeValue { S = "admin@example.com" } },
                { "Password", new AttributeValue { S = PasswordHasher.HashPassword("password2") } },
                { "Role", new AttributeValue { S = "Admin" } }
            }
        };

        foreach (var user in users)
        {
            try
            {
                var putItemRequest = new PutItemRequest
                {
                    TableName = tableName,
                    Item = user
                };

                await _dynamoDb.PutItemAsync(putItemRequest);
                Console.WriteLine($"User '{user["Name"].S}' inserted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting user '{user["Name"].S}': {ex.Message}");
            }
        }
    }
}
