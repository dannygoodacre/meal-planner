using DannyGoodacre.Cqrs;
using Microsoft.EntityFrameworkCore.Storage;

namespace MealPlanner.Data;

public sealed class Transaction(IDbContextTransaction transaction) : ITransaction
{
    public async Task CommitAsync(CancellationToken cancellationToken = default)
        => await transaction.CommitAsync(cancellationToken);

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
        => await transaction.RollbackAsync(cancellationToken);

    public async ValueTask DisposeAsync()
        => await transaction.DisposeAsync();
}
