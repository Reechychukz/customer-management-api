using Microsoft.AspNetCore.Mvc;
using Application.Services.Interfaces;
using Application.DTOs;
using Application.Helpers;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BankController: ControllerBase
	{
		private readonly IBankService _bankService;
        private readonly ILogger<IUserService> _logger;

        public BankController(IBankService bankService, ILogger<IUserService> logger)
        {
            _bankService = bankService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint to get all existing banks from the provided test API
        /// </summary>
        /// <returns>A list of banks existing</returns>
        [HttpGet]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<ExistingBankDto>>), 200)]
        public async Task<IActionResult> GetAllExistingBanks()
        {
            try
            {
                var response = await _bankService.GetExistingBanks();

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

