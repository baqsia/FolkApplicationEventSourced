using FolkApplication.Domain.Song;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Microsoft.EntityFrameworkCore;

namespace FolkApplication.Console;

public class SongProjectionViewStream(DbContext dbContext) : IProjection
{
    private async Task CopyAsDocument(StreamAction stream, CancellationToken cancellation)
    {
        var aggregate = new Song();
        foreach (var @event in stream.Events)
        {
            aggregate.Apply((dynamic)@event.Data);
            aggregate.Version = @event.Version;
        }

        await dbContext.Set<Song>().AddAsync(aggregate, cancellation);
        await dbContext.SaveChangesAsync(cancellation);
    }

    public void Apply(IDocumentOperations operations, IReadOnlyList<StreamAction> streams)
    {
        throw new NotImplementedException();
    }

    public async Task ApplyAsync(IDocumentOperations operations, IReadOnlyList<StreamAction> streams,
        CancellationToken cancellation)
    {
        foreach (var stream in streams)
        {
            var view = await dbContext.Set<Song>()
                .FirstOrDefaultAsync(a => a.Id == stream.Id && a.Version < stream.Version,
                    cancellationToken: cancellation);
            if (view is not null) continue;

            await CopyAsDocument(stream, cancellation);
        }
    }
}