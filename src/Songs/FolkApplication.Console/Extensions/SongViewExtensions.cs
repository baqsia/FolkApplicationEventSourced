using FolkApplication.Domain.Song.Events;

namespace FolkApplication.Console.Extensions;

public static class SongViewExtensions
{
    public static SongView AsView(this SongCreatedEvent e)
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
    }
}