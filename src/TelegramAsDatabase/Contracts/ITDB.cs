using FluentResults;
using TelegramAsDatabase.Models;

namespace TelegramAsDatabase.Contracts;

public interface ITDB
{
    Task<Result<TDBData<T>>> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task<Result<List<string>>> GetAllKeysAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync<T>(string key, CancellationToken cancellationToken = default);
    Task<Result> SaveAsync<T>(TDBData<T> item, CancellationToken cancellationToken = default);
    Task<Result> SaveAsync<T>(IEnumerable<TDBData<T>> items, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync<T>(string key, TDBData<T> value, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(string key, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);
    Task<Result> ClearAsync(CancellationToken cancellationToken = default);
}