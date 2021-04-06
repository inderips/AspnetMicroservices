using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            using (var scope = host.Services.CreateScope())
            {
                var configuration = scope.ServiceProvider.GetService<IConfiguration>();
                var logger = scope.ServiceProvider.GetService<ILogger<TContext>>();
                try
                {
                    using (var connection = new Npgsql.NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString")))
                    {
                        connection.Open();
                        using (var command = new Npgsql.NpgsqlCommand())
                        {
                            command.Connection = connection;
                            command.CommandText = "DROP TABLE IF EXISTS \"Coupon\"";
                            command.ExecuteNonQuery();

                             command.CommandText = "CREATE TABLE \"Coupon\"(\"ID\" SERIAL PRIMARY KEY,\"ProductName\" VARCHAR(24) NOT NULL, \"Description\" TEXT, \"Amount\" INT)";
                            command.ExecuteNonQuery();

                            command.CommandText = "INSERT INTO \"Coupon\" (\"ProductName\",\"Description\",\"Amount\") VALUES ('IPhone X','IPhone Discount', 150)";
                            command.ExecuteNonQuery();

                            command.CommandText = "INSERT INTO \"Coupon\" (\"ProductName\",\"Description\",\"Amount\") VALUES ('Samsung 10','Samsung Discount', 100)";
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Npgsql.NpgsqlException exception)
                {
                    logger.LogError(exception.Message);
                    MigrateDatabase<TContext>(host, retryForAvailability++);
                }
            }

            return host;
        }
    }
}
    

