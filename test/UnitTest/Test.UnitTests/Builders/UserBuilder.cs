using Bogus;
using Test.UnitTests.Models;

namespace Test.UnitTests.Builders;

public static class UserBuilder
{
    private static readonly Faker Faker = new("en");

    public static User Build()
    {
        return new User()
        {
            Id = Faker.Random.Int(),
            Name = Faker.Name.FullName(),
            Email = Faker.Internet.Email(),
            Password = Faker.Internet.Password(),
            PhoneNumber = Faker.Phone.PhoneNumber()
        };
    }

    public static List<User> Build(int count)
    {
        var users = new List<User>();

        for (var i = 0; i < count; i++)
            users.Add(Build());

        return users;
    }
}