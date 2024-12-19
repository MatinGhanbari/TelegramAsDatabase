using Bogus;
using System.Linq.Expressions;
using System.Reflection;
using Test.UnitTests.Models;

namespace Test.UnitTests.Builders;

public class UserBuilder
{
    private static readonly Faker Faker = new("en");
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public UserBuilder()
    {
        Id = Faker.Random.Int();
        Name = Faker.Name.FullName();
        Email = Faker.Internet.Email();
        Password = Faker.Internet.Password();
        PhoneNumber = Faker.Phone.PhoneNumber();
    }

    public UserBuilder With<T>(Expression<Func<UserBuilder, T>> prop, T value)
    {
        if (prop.Body is MemberExpression memberExpression)
        {
            if (memberExpression.Member is PropertyInfo propertyInfo)
            {
                propertyInfo.SetValue(this, value);
            }
        }

        return this;
    }

    public User Build()
    {
        return new User()
        {
            Id = Id,
            Name = Name,
            Email = Email,
            Password = Password,
            PhoneNumber = PhoneNumber
        };
    }

    public List<User> Build(int count)
    {
        return Enumerable.Range(0, count).Select(_ => Build()).ToList();
    }
}