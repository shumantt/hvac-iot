using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServiceLayerApi.CommandProcessing;
using ServiceLayerApi.CommandProcessing.Models;

namespace ServiceLayerApi.Controllers.Commands
{
    [ApiController]
    [Route("commands")]
    public class CommandsController : ControllerBase
    {
        private readonly CommandProcessingService _commandProcessingService;

        public CommandsController(CommandProcessingService commandProcessingService)
        {
            _commandProcessingService = commandProcessingService;
        }

        [HttpPost]
        [Route("decision")]
        public async Task<ActionResult<DecisionCommandProcessResult>> ProcessDecisionCommand([FromBody] DecisionCommand decisionCommand)
        {
            var result = await _commandProcessingService.ProcessDecisionCommand(decisionCommand).ConfigureAwait(false);
            return result;
        }
    }
}