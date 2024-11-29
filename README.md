# Movie reviews

## About
This is a .NET API for users to review movies. There are 3 types of users:
- Guest (unauthorised): can list movies
- Regular user: same as guest and also can add reviews
- Admin users: same as regular and can also CRUD movies

## Requirements
To be able to run the app you need a DynamoDb instance in the cloud. You also need a file in the app's root called "aws_credentials.json". This file must contain this 3 keys:
- AwsAccessKeyId
- AwsSecretAccessKey
- Region

## Installation
Each time the app runs it will try to create 3 tables in the DynamoDb instance:
- Users
- Movies
- Reviews
It will also add 2 users: a regular user and an admin. The credentials for each user can be found in the class DynamoDbInitializer.