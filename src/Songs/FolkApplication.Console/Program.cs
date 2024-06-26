﻿using Elastic.Clients.Elasticsearch;
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
    .AddEventViewContext()
    .AddMarten((sp) =>
    {
        var options = new StoreOptions();
        options.Connection(sql);
        options.UseSystemTextJsonForSerialization();

        options.Events.DatabaseSchemaName = "song_events";
        options.Events.StreamIdentity = StreamIdentity.AsGuid;

        options.Projections.LiveStreamAggregation<Song>();
        options.Projections.LiveStreamAggregation<Singer>();
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

var maroon5 = Singer.New("Maroon 5", true);
var rammstein = Singer.New("Rammstein", true);
var duaLipa = Singer.New("Dua Lipa");

await Task.WhenAll(
    domainEventRepo.Store(maroon5, tokenSource.Token),
    domainEventRepo.Store(rammstein, tokenSource.Token),
    domainEventRepo.Store(duaLipa, tokenSource.Token)
);
var song = Song.New("Du Hast", maroon5, rammstein, duaLipa);
await domainEventRepo.Store(song, tokenSource.Token);

// var songs = await domainViewRepo.QueryAsync<SongView>();

var songView =
    await domainViewRepo.QueryFirstAsync<SongView>(song.Id,
        tokenSource.Token);

song = await domainEventRepo.FindProjection<Song>(song.Id, song.Version, tokenSource.Token);
song!.Rate(10);
await domainEventRepo.UpdateAsync(song, tokenSource.Token);

songView = await domainViewRepo.QueryFirstAsync<SongView>(
    song.Id,
    tokenSource.Token
);

song.AddLyrics(@"25 years and my life is still
Tryin' to get up that great big hill of hope
For a destination
I realized quickly when I knew I should
That the world was made up of this brotherhood of man
For whatever that means");
await domainEventRepo.UpdateAsync(song, tokenSource.Token);
// var stream = await domainEventRepo.StreamByIdAsync(song.Id, song.Version, CancellationToken.None);
// var aggregate = await domainEventRepo.FindProjection<Song>(song.Id, song.Version, CancellationToken.None);


Console.CancelKeyPress += async (sender, eventArgs) => { await tokenSource.CancelAsync(); };
Console.ReadKey();