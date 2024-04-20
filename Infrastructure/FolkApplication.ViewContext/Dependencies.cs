using FolkApplication.Framework;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FolkApplication.ViewContext;

public static class Dependencies
{
    public static IServiceCollection AddEventViewContext(
        this IServiceCollection services
    )
    {
        //"mongodb://<username>:<password>@<hostname>:<port>/?authSource=<authenticationDb>"
        const string connectionString = "mongodb+srv://baqsiaa:VVdKpRDnMHuCTRAZ@cluster0.gs7cpgw.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
        const string db = "songs_view";
        
        services
            .AddSingleton<IMongoClient, MongoClient>(sp =>
            {
                var client = new MongoClient(connectionString);
                client.GetDatabase(db);
                
                return client;
            })
            .AddTransient<IMongoDatabase>(sp =>
            {
                var mongoClient = sp.GetRequiredService<IMongoClient>();
                var database = mongoClient.GetDatabase(db);
                database.CreateCollectionAsync("songs");
                return database;
            })
            .AddTransient<IDomainViewRepository, DomainViewRepository>();
        return services;
    }
}