using FluentResults;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAsDatabase.Contracts;
using TelegramAsDatabase.Models;

namespace TelegramAsDatabase.Implementations;

public class TDBTransaction : ITDBTransaction
{
    private readonly ITDB _tdb;
    private readonly int _tdbIndexMessage;
    private readonly List<Func<Task<Result>>> _actions = new();

    public TDBTransaction(ITDB tdb, int tdbIndexMessage)
    {
        _tdb = tdb;
        _tdbIndexMessage = tdbIndexMessage;
    }

    public async Task<Result> SaveAsync<T>(TDBData<T> item, CancellationToken cancellationToken = default)
    {
        try
        {
            _actions.Add(() => _tdb.SaveAsync(item, cancellationToken));
            return Result.Ok();
        }
        catch (Exception exception)
        {
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result> SaveAsync<T>(IEnumerable<TDBData<T>> items, CancellationToken cancellationToken = default)
    {
        try
        {
            var action = () => _tdb.SaveAsync(items, cancellationToken);

            _actions.Add(action);
            return Result.Ok();
        }
        catch (Exception exception)
        {
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result> UpdateAsync<T>(string key, TDBData<T> value, CancellationToken cancellationToken = default)
    {
        try
        {
            var action = () => _tdb.UpdateAsync(key, value, cancellationToken);

            _actions.Add(action);
            return Result.Ok();
        }
        catch (Exception exception)
        {
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result> DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var action = () => _tdb.DeleteAsync(key, cancellationToken);

            _actions.Add(action);
            return Result.Ok();
        }
        catch (Exception exception)
        {
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result> DeleteAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        try
        {
            var action = () => _tdb.DeleteAsync(keys, cancellationToken);

            _actions.Add(action);
            return Result.Ok();
        }
        catch (Exception exception)
        {
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result> ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var action = () => _tdb.ClearAsync(cancellationToken);

            _actions.Add(action);
            return Result.Ok();
        }
        catch (Exception exception)
        {
            return Result.Fail(exception.Message);
        }
    }

    public async Task<Result> CommitAsync(CancellationToken cancellationToken = default)
    {
        var tdb = (TDB)_tdb;
        await tdb.RecreateIndex();

        foreach (var action in _actions)
        {
            var res = await action.Invoke();
            if (res.IsFailed)
                throw new Exception(res.Errors.First().Message);
        }

        return Result.Ok();
    }

    public async Task<Result> Rollback(CancellationToken cancellationToken = default)
    {
        try
        {
            var tdb = (TDB)_tdb;
            await tdb.RollbackIndex(_tdbIndexMessage);

            return Result.Ok();
        }
        catch (Exception exception)
        {
            return Result.Fail(exception.Message);
        }
    }

    public void Dispose()
    {

    }
}