using FolkApplication.Domain.Song;
using FolkApplication.Domain.Song.Events;
using FolkApplication.Framework;
using Marten;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;

namespace FolkApplication.Console;

public class SongProjectionViewStream : EventProjection
{
    private readonly IDomainViewRepository _viewRepository;

    public SongProjectionViewStream(IDomainViewRepository viewRepository)
    {
        _viewRepository = viewRepository;
        ProjectAsync<SongCreatedEvent>(async (e, op) =>
        {
            await GetAndUpdate(e.SongId, view =>
            {
                view.Name = e.Name;
                view.Version = 1;
                return view;
            });
        });

        ProjectAsync<RateSongEvent>(async (e, op) =>
        {
            await GetAndUpdate(e.SongId, view =>
            {
                view.Rating = e.Rating;
                return view;
            });
        });
    }

    private async Task GetAndUpdate(Guid streamId, Func<SongView, SongView> onView)
    {
        var view = await _viewRepository.QueryFirstAsync<SongView>(streamId, CancellationToken.None);
        await _viewRepository.UpdateAsync(onView(view!), CancellationToken.None);
    }

    // public async Task ApplyAsync(IDocumentOperations operations, IReadOnlyList<StreamAction> streams,
    //     CancellationToken cancellation)
    // {
    //     foreach (var stream in streams)
    //     {
    //         var view = await viewRepository.QueryFirstAsync<SongView>(stream.Id,
    //             cancellationToken: cancellation);
    //         if (view is not null)
    //         {
    //             if (view.Version < stream.Version)
    //                 await CopyAsDocument(view, stream, cancellation);
    //
    //             continue;
    //         }
    //
    //         await InitialCopyOfDocument(stream, cancellation);
    //     }
    // }
}