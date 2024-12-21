using FluentResults;
using Microsoft.AspNetCore.Mvc;
using TelegramAsDatabase.Contracts;
using TelegramAsDatabase.Models;
using Test.Integration.WebApi.Models;

namespace Test.Integration.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TDBIntegrationTestController : ControllerBase
    {
        private readonly ITDB _tdb;
        private readonly ILogger<TDBIntegrationTestController> _logger;

        public TDBIntegrationTestController(ILogger<TDBIntegrationTestController> logger, ITDB tdb)
        {
            _logger = logger;
            _tdb = tdb;
        }

        [HttpPost("Transaction")]
        public async Task<IActionResult> TestTransaction(CancellationToken cancellationToken)
        {
            using var transaction = (await _tdb.BeginTransaction(cancellationToken)).Value;
            try
            {
                var user1 = new User() { Name = "User - 1" };
                var user2 = new User() { Name = "User - 2" };

                await transaction.SaveAsync(new TDBData<User>() { Key = "1", Value = user1 }, cancellationToken);
                await transaction.SaveAsync(new TDBData<User>() { Key = "2", Value = user2 }, cancellationToken);

                await transaction.DeleteAsync("2", cancellationToken);

                user2.Email = "Email";
                await transaction.SaveAsync(new TDBData<User>() { Key = "2", Value = user2 }, cancellationToken);

                user1.Email = "user1@mail.com";
                user2.Email = "user2@mail.com";
                await transaction.UpdateAsync("11", new TDBData<User>() { Key = "1", Value = user1 }, cancellationToken);
                await transaction.UpdateAsync("2", new TDBData<User>() { Key = "2", Value = user2 }, cancellationToken);

                var result = await transaction.CommitAsync(cancellationToken);

                return result.IsFailed ? BadRequest("Not Found") : Ok(result);
            }
            catch (Exception exception)
            {
                await transaction.Rollback(cancellationToken);
                throw;
            }
        }

        [HttpGet("Users")]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            var result = await _tdb.GetAllKeysAsync(cancellationToken);
            return result.IsFailed ? BadRequest("Not Found") : Ok(result.Value);
        }

        [HttpGet("Users/{id}")]
        public async Task<IActionResult> GetUserAsync([FromRoute] string id, CancellationToken cancellationToken)
        {
            var result = await _tdb.GetAsync<User>(id, cancellationToken);
            return result.IsFailed ? BadRequest("Not Found") : Ok(result.Value);
        }

        [HttpPost("Users/{id}")]
        public async Task<IActionResult> CreateUserAsync([FromRoute] string id, [FromBody] User user, CancellationToken cancellationToken)
        {
            var result = await _tdb.SaveAsync<User>(new TDBData<User>()
            {
                Key = id,
                Value = user
            }, cancellationToken);
            return result.IsFailed ? BadRequest(result.Errors) : Ok();
        }

        [HttpPut("Users/{id}")]
        public async Task<IActionResult> UpdateUserAsync([FromRoute] string id, [FromBody] User user, CancellationToken cancellationToken)
        {
            var result = await _tdb.UpdateAsync(id, new TDBData<User>()
            {
                Key = id,
                Value = user
            }, cancellationToken);
            return result.IsFailed ? BadRequest(result.Errors) : Ok();
        }

        [HttpDelete("Users")]
        public async Task<IActionResult> CreateUserAsync([FromBody] List<string> ids, CancellationToken cancellationToken)
        {
            Result result;

            if (ids == null || ids.Count == 0)
                result = await _tdb.ClearAsync(cancellationToken);
            else
                result = await _tdb.DeleteAsync(ids, cancellationToken);

            return result.IsFailed ? BadRequest(result.Errors) : Ok();
        }
    }
}
