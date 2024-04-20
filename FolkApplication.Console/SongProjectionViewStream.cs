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
            await GetAndUpdate(e.SongId, _ =>
            {
                var view = new SongView
                {
                    Name = e.Name,
                    Version = e.Version,
                    Singers = e.Singers.Select(singer => new SingerView
                    {
                        Name = singer.Name,
                        Songs = singer.Songs.Select(song => new SongView
                        {
                            Name = song.Name,
                            Version = song.Version,
                            Id = song.Id.ToString()
                        }).ToList(),
                        IsBand = singer.IsBand
                    }).ToList(),
                    Id = e.SongId.ToString()
                };
                return view;
            });
        });

        ProjectAsync<RateSongEvent>(async (e, op) =>
        {
            await GetAndUpdate(e.SongId, view =>
            {
                view!.Rating = e.Rating;
                view.Version = e.Version;
                return view;
            });
        });
        
        ProjectAsync<LyricAddedEvent>(async (e, op) =>
        {
            await GetAndUpdate(e.SongId, view =>
            {
                view!.Lyrics.Add(new Lyric(e.Value)); 
                view.Version = e.Version;
                return view;
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