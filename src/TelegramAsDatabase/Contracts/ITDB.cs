using FluentResults;
using TelegramAsDatabase.Models;

namespace TelegramAsDatabase.Contracts;

public interface ITDB
{
    Task<Result<TDBData<T>>> GetAsync<T>(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync<T>(Guid id, CancellationToken cancellationToken = default);
    Task<Result> SaveAsync<T>(TDBData<T> data, CancellationToken cancellationToken = default);
    Task<Result> ClearAsync(CancellationToken cancellationToken = default);
}