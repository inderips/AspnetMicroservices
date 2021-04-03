using System;
using Catalog.API.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext :ICatalogContext
    {
        private readonly ILogger<CatalogContext> _logger;
        public CatalogContext(ILogger<CatalogContext> logger, IConfiguration configuration)
        {
            _logger = logger;
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            _logger.LogInformation($"Client details -> {configuration.GetValue<string>("DatabaseSettings:ConnectionString")}");
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));
            _logger.LogInformation($"Database details -> {configuration.GetValue<string>("DatabaseSettings:DatabaseName")}");
            Products = database.GetCollection<Product>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));

            CatalogContextSeed.SeedData(Products);
        }

        public  IMongoCollection<Product> Products { get; }
    }
}
