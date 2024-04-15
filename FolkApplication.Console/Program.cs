using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Transport;
using FolkApplication.Console;
using FolkApplication.Domain.Song;
using FolkApplication.Framework;
using FolkApplication.Infrastructure;
using FolkApplication.ViewContext;
using Marten;
using Marten.Events;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


var tokenSource = new CancellationTokenSource();

const string sql = "Host=localhost;Port=5434;User ID=admin;Password=adminpass;Database=folk-songs;";
var services = new ServiceCollection();
services.AddSingleton<IDomainEventRepository, DomainEventRepository>()
    .AddTransient<IDomainViewRepository, DomainViewRepository>()
    .AddMarten((sp) =>
    {
        var options = new StoreOptions();
        options.Connection(sql);
        options.UseSystemTextJsonForSerialization();

        options.Events.DatabaseSchemaName = "song_events";
        options.Events.StreamIdentity = StreamIdentity.AsGuid;

        options.Projections.LiveStreamAggregation<Song>();
        options.Projections.Add(new SongProjectionViewStream(sp.GetRequiredService<IDomainViewRepository>()), ProjectionLifecycle.Inline);

        return options;
    }).AddAsyncDaemon(DaemonMode.Solo);
services.AddDbContextPool<SongDbContext>(opt => { opt.UseNpgsql(sql); });
services.AddSingleton<ElasticsearchClient>(sp =>
{
    var settings = new ElasticsearchClientSettings(
        "personal_learning:dXMtY2VudHJhbDEuZ2NwLmNsb3VkLmVzLmlvJGQzYmY1ZGZhNTY2NzQ0OGQ5NzAyNmQwZTU0M2E4YjE5JDRiNDYwOGM4YmNlYTQ4ODI5NGUwNDU0ZjZmOTdjNjMw",
        new BasicAuthentication("elastic", "jQaXkxkzGlksutf4DRyVHtRr"));

    var client = new ElasticsearchClient(settings);
    client.Indices.CreateAsync(ElasticIndices.SongIndex, def =>
        {
            def.Mappings(map =>
            {
                map.Properties(new Properties
                {
                    { "id", new TextProperty() },
                    { "name", new TextProperty() },
                    { "version", new LongNumberProperty() }
                });
            });
        })
        .GetAwaiter().GetResult();
    return client;
});


await using var provider = services.BuildServiceProvider();

var domainEventRepo = provider.GetRequiredService<IDomainEventRepository>();
var domainViewRepo = provider.GetRequiredService<IDomainViewRepository>();

// var song = Song.Create("whats up with that");
// await domainEventRepo.Store(song, tokenSource.Token);

// var songs = await domainViewRepo.QueryAsync<SongView>();
 
var song = await domainViewRepo.QueryFirstAsync<SongView>(Guid.Parse("4f0b17d9-a9b5-4c84-a805-38d1adcab100"), tokenSource.Token);

var songAggregate = await domainEventRepo.FindProjection<Song>(song!.Id, song.Version, tokenSource.Token);
songAggregate!.Rate(10);
await domainEventRepo.UpdateAsync(songAggregate, tokenSource.Token);

// var stream = await domainEventRepo.StreamByIdAsync(song.Id, song.Version, CancellationToken.None);
// var aggregate = await domainEventRepo.FindProjection<Song>(song.Id, song.Version, CancellationToken.None);


Console.CancelKeyPress += async (sender, eventArgs) => { await tokenSource.CancelAsync(); };
Console.ReadKey();