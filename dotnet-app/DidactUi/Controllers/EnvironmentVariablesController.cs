using Microsoft.AspNetCore.Mvc;

namespace DidactUi.Controllers
{
    public class EnvironmentVariablesController : ControllerBase
    {
        private readonly ILogger<EnvironmentVariablesController> _logger;

        public EnvironmentVariablesController(ILogger<EnvironmentVariablesController> logger)
        {
            _logger = logger;
        }

        [HttpGet("environment-variables")]
        public IActionResult GetEnvironmentVariables()
        {
            var envVariables = new { test = "Dummy value for Nuxt." };
            return Ok(envVariables);
        }
    }
}
