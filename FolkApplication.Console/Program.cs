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
        options.Projections.Add(new SongProjectionViewStream(sp.GetRequiredService<SongDbContext>()),
            ProjectionLifecycle.Inline);

        return options;
    }).AddAsyncDaemon(DaemonMode.Solo);
services.AddDbContextPool<SongDbContext>(opt => { opt.UseNpgsql(sql); });

await using var provider = services.BuildServiceProvider();

var domainEventRepo = provider.GetRequiredService<IDomainEventRepository>();
var domainViewRepo = provider.GetRequiredService<IDomainViewRepository>();
//
// var song = Song.Create("whats up with that");
// await domainEventRepo.Store(song, tokenSource.Token);

var aggregates = await domainViewRepo.QueryPaged<Song>(1, 20, tokenSource.Token);


// var stream = await domainEventRepo.StreamByIdAsync(song.Id, song.Version, CancellationToken.None);
// var aggregate = await domainEventRepo.FindProjection<Song>(song.Id, song.Version, CancellationToken.None);


Console.CancelKeyPress += async (sender, eventArgs) => { await tokenSource.CancelAsync(); };
Console.ReadKey();