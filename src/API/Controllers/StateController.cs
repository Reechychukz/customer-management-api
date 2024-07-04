using Microsoft.AspNetCore.Mvc;
using Application.Services.Interfaces;
using Application.DTOs;
using Application.Helpers;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StateController: ControllerBase
	{
		private readonly IStateService _stateService;
        private readonly ILogger<IUserService> _logger;

        public StateController(IStateService stateService, ILogger<IUserService> logger)
        {
            _stateService = stateService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint to get all States
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A list of LGAs corresponding to the state id</returns>
        [HttpGet]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<StateDto>>), 200)]
        public async Task<IActionResult> GetAllStates()
        {
            try
            {
                var response = await _stateService.GetStates();

                return Ok(response);
            }
            catch (RestException ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest(new Response(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new Response(ex.Message));
            }

        }

        /// <summary>
        /// Endpoint to get LGAs by State Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A list of LGAs corresponding to the state id</returns>
        [HttpGet("{stateId}")]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<LGADto>>), 200)]
        public async Task<IActionResult> GetLGAsByStateId(int stateId)
        {
            try
            {
                var response = await _stateService.GetAllLGAByStateId(stateId);

                return Ok(response);
            }
            catch (RestException ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest(new Response(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new Response(ex.Message));
            }

        }
    }
}

