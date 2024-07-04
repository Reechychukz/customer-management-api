using Application.Helpers;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    public record Response(string Message);
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<IUserService> _logger;

        public AuthController(IUserService userService, ILogger<IUserService> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint to register a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //[AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(SuccessResponse<UserDto>), 201)]
        public async Task<IActionResult> RegisterUser(UserSignupDto model)
        {
            try
            {
                var response = await _userService.CreateUser(model);

                return CreatedAtAction(nameof(GetUserById), new { id = response.Data.Id }, response);
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
        /// Endpoint to verify a user phone number
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("complete-onboarding")]
        [ProducesResponseType(typeof(SuccessResponse<object>), 201)]
        public async Task<IActionResult> ComfirmUserPhoneNumber(VerifyTokenDTO model)
        {
            try
            {
                var response = await _userService.CompleteUserOnboarding(model);

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
        /// Endpoint to login as a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(SuccessResponse<UserLoginResponse>), 200)]
        public async Task<IActionResult> LoginUser(UserLoginDTO model)
        {
            try
            {
                var response = await _userService.UserLogin(model);

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
        /// Endpoint to get a user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetUserById))]
        [ProducesResponseType(typeof(SuccessResponse<UserByIdDto>), 200)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var response = await _userService.GetUserById(id);
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
        /// Endpoint to get a paginated list of all existing customers previously onboarded
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetUsers))]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<UserDto>>), 200)]
        public async Task<IActionResult> GetUsers([FromQuery] ResourceParameter parameter)
        {
            try
            {
                var response = await _userService.GetUsers(parameter, nameof(UserDto), Url);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new Response(ex.Message));
            }
        }
    }
}

