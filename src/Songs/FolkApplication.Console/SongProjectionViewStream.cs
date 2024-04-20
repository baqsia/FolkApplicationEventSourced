using FolkApplication.Console.Extensions;
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
        ProjectAsync<SongCreatedEvent>(async (e, op) => { await GetAndUpdate(e.SongId, _ => e.AsView()); });

        ProjectAsync<RateSongEvent>(async (e, op) =>
        {
            await GetAndUpdate(e.SongId, view => view! with
            {
                Rating = e.Rating,
                Version = e.Version
            });
        });

        ProjectAsync<LyricAddedEvent>(async (e, op) =>
        {
            await GetAndUpdate(e.SongId, view => view! with
            {
                Version = e.Version,
                Lyrics = [..view.Lyrics, new Lyric(e.Value)]
            });
        });
    }

    private async Task GetAndUpdate(Guid streamId, Func<SongView?, SongView> onView)
    {
        var view = await _viewRepository.QueryFirstAsync<SongView>(streamId, CancellationToken.None);
        if (view is not null)
        {
            await _viewRepository.UpdateAsync(onView(view), CancellationToken.None);
            return;
        }

        await _viewRepository.PushAsync(onView(view), CancellationToken.None);
    }
}