using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAsDatabase.Models;

namespace TelegramAsDatabase.Contracts;

public interface ITDBTransaction : IDisposable
{
    /// <summary>
    /// Saves a single item of type TDBData<T> to the data store.
    /// </summary>
    /// <typeparam name="T">The type of the data being saved.</typeparam>
    /// <param name="item">The item to save to the data store.</param>
    /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
    /// <returns>A task that resolves to a Result indicating whether the save operation was successful or failed.</returns>
    Task<Result> SaveAsync<T>(TDBData<T> item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves multiple items of type TDBData<T> to the data store.
    /// </summary>
    /// <typeparam name="T">The type of the data being saved.</typeparam>
    /// <param name="items">The collection of items to save to the data store.</param>
    /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
    /// <returns>A task that resolves to a Result indicating whether the save operation was successful or failed.</returns>
    Task<Result> SaveAsync<T>(IEnumerable<TDBData<T>> items, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing item in the data store with the provided key and value.
    /// </summary>
    /// <typeparam name="T">The type of the data being updated.</typeparam>
    /// <param name="key">The key of the item to update.</param>
    /// <param name="value">The new value to update the item with.</param>
    /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
    /// <returns>A task that resolves to a Result indicating whether the update operation was successful or failed.</returns>
    Task<Result> UpdateAsync<T>(string key, TDBData<T> value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an item from the data store based on the provided key.
    /// </summary>
    /// <param name="key">The key of the item to delete.</param>
    /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
    /// <returns>A task that resolves to a Result indicating whether the delete operation was successful or failed.</returns>
    Task<Result> DeleteAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes multiple items from the data store based on the provided collection of keys.
    /// </summary>
    /// <param name="keys">A collection of keys of the items to delete.</param>
    /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
    /// <returns>A task that resolves to a Result indicating whether the delete operation was successful or failed.</returns>
    Task<Result> DeleteAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all data from the data store.
    /// </summary>
    /// <param name="cancellationToken">Optional. A token to monitor for cancellation requests.</param>
    /// <returns>A task that resolves to a Result indicating whether the clear operation was successful or failed.</returns>
    Task<Result> ClearAsync(CancellationToken cancellationToken = default);

    Task<Result> CommitAsync(CancellationToken cancellationToken = default);
    Task<Result> Rollback(CancellationToken cancellationToken = default);
}