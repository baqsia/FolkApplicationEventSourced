using FolkApplication.Framework;
using Microsoft.EntityFrameworkCore;

namespace FolkApplication.ViewContext;

public class DomainViewRepository(SongDbContext dbContext) : IDomainViewRepository
{
    public async Task<IEnumerable<TAggregate>?> QueryPaged<TAggregate>(int pageNumber = 1, int pageSize = 20,
        CancellationToken cancellationToken = default)
        where TAggregate : AggregateRoot
    {
        return await dbContext.Set<TAggregate>()
            .Skip((pageNumber - 1) * 20)
            .Take(pageSize)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}