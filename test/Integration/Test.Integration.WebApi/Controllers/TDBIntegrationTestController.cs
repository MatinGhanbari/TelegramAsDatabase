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

        [HttpDelete("Users/{id}")]
        public async Task<IActionResult> CreateUserAsync([FromRoute] string id, CancellationToken cancellationToken)
        {
            var result = await _tdb.DeleteAsync(id, cancellationToken);
            return result.IsFailed ? BadRequest(result.Errors) : Ok();
        }
    }
}
