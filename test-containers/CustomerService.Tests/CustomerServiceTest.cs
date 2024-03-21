using System.Data.Common;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Customers.Tests;

public sealed class CustomerServiceTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16.2-alpine")
        .Build();

    public Task InitializeAsync()
    {
        return _postgres.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    [Fact]
    public void ShouldReturnTwoCustomers()
    {
        // Given
        var customerService = new CustomerService(new DbConnectionProvider(_postgres.GetConnectionString()));

        // When
        customerService.Create(new Customer(1, "George"));
        customerService.Create(new Customer(2, "John"));
        var customers = customerService.GetCustomers();

        // Then
        Assert.Equal(2, customers.Count());
    }
    
    [Fact]
    public void ExecuteCommand()
    {
        using (DbConnection connection = new NpgsqlConnection(_postgres.GetConnectionString()))
        {
            using (DbCommand command = new NpgsqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT 1";
            }
        }
    }
}