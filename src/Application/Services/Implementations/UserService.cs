using Domain.Entities;
using Application.Services.Interfaces;
using Infrastructure.Repositories.Interfaces;
using Application.Helpers;
using Application.DTOs;
using AutoMapper;
using Domain.Enums;
using Domain.Common;
using Microsoft.AspNetCore.Identity;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.QueryableExtensions;
using System.Net.Http.Headers;

namespace Application.Services.Implementations
{
	public class UserService: IUserService
	{
        private readonly IUserRepository _userRepository;
        private readonly IRepository<UserActivity> _userActivityRepository;
        private readonly IRepository<Token> _tokenRepository;
        private readonly IRepository<State> _stateRepository;
        private readonly IRepository<LGA> _lgaRepository;
        private readonly IMapper _mapper;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        private readonly HttpClient _httpCLient;

        public UserService(
            IUserRepository userRepository,
            IRepository<UserActivity> userActivityRepository,
            IRepository<Token> tokenRepository,
            IMapper mapper,
            RoleManager<Role> roleManager,
            UserManager<User> userManager,
            IJwtAuthenticationManager jwtAuthenticationManager,
            IRepository<State> stateRepository,
            IRepository<LGA> lgaRepository,
            HttpClient httpCLient)
        {
            _userRepository = userRepository;
            _userActivityRepository = userActivityRepository;
            _tokenRepository = tokenRepository;
            _mapper = mapper;
            _roleManager = roleManager;
            _userManager = userManager;
            _jwtAuthenticationManager = jwtAuthenticationManager;
            _stateRepository = stateRepository;
            _lgaRepository = lgaRepository;
            _httpCLient = httpCLient;
        }

        /// <summary>
        /// This method creates a user and fake sends an otp to the console in which the user can use in verifying their phone number
        /// Every user is given the user role but only the seeded user is given the Admin role
        /// Activities of users are also saved for security purposes
        /// </summary>
        /// <param name="model"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        /// <exception cref="RestException"></exception>
        public async Task<SuccessResponse<UserDto>> CreateUser(UserSignupDto model, List<string> roles = null)
        {
            var email = model.Email.Trim().ToLower();
            var isEmailExist = await _userRepository.ExistsAsync(x => x.Email == email);

            if (isEmailExist)
                throw new RestException(HttpStatusCode.BadRequest, message: ResponseMessages.DuplicateEmail);

            var phoneNumber = model.PhoneNumber.Trim();
            var isPhoneNumberExist = await _userRepository.ExistsAsync(x => x.PhoneNumber == phoneNumber);
            if (isPhoneNumberExist)
                throw new RestException(HttpStatusCode.BadRequest, message: ResponseMessages.DuplicatePhoneNumber);

            if (string.IsNullOrEmpty(model.Password))
                throw new RestException(HttpStatusCode.BadGateway, message: ResponseMessages.PasswordCannotBeEmpty);

            var user = _mapper.Map<User>(model);
            user.UserName = user.Email;
            user.IsVerified = false;

            var state = await _stateRepository.FirstOrDefault(x => x.Id == model.StateId);
            if (state == null)
                throw new RestException(HttpStatusCode.NotFound, "State not found");

            var lga = await _lgaRepository.FirstOrDefault(x => x.Id == model.LGAId);
            if (lga == null || lga.StateId != model.StateId)
                throw new RestException(HttpStatusCode.NotFound, "LGA not found or does not belong to the selected State");
            

            if (roles is null)
            {
                roles = new List<string>();
                var role = await _roleManager.FindByNameAsync(ERole.USER.ToString());
                if (role is not null)
                    roles.Add(role.Name);
            }

            await _userManager.CreateAsync(user, model.Password);

            await AddUserToRoles(user, roles);

            var userActivity = new UserActivity
            {
                EventType = "User created",
                UserId = user.Id,
                ObjectClass = "USER",
                Details = "signed up",
                ObjectId = user.Id
            };

            await _userActivityRepository.AddAsync(userActivity);

            var token = CustomToken.GenerateOtp();

            //send comfirmation token using a fake smtp fake
            var tokenEntity = new Token
            {
                UserId = user.Id,
                TokenType = TokenTypeEnum.PHONENUMBER_COMFIRMATION,
                OTPToken = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsValid = true
            };
            await _tokenRepository.AddAsync(tokenEntity);

            // Mock sending OTP
            string otp = tokenEntity.OTPToken;
            Console.WriteLine($"Mock OTP sent to {user.PhoneNumber}: {otp}");

            await _userActivityRepository.SaveChangesAsync();

            var userResponse = _mapper.Map<UserDto>(user);
            userResponse.State = state.Name;
            userResponse.LGA = lga.Name;

            return new SuccessResponse<UserDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = userResponse
            };
        }

        /// <summary>
        /// This methos is used to login a user using their email and password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="RestException"></exception>
        public async Task<SuccessResponse<UserLoginResponse>> UserLogin(UserLoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.WrongEmailOrPassword);

            var isUserValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isUserValid)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.WrongEmailOrPassword);

            //user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var userActivity = new UserActivity
            {
                EventType = "User Login",
                UserId = user.Id,
                ObjectClass = "USER",
                Details = "logged in",
                ObjectId = user.Id
            };

            var roles = await _userManager.GetRolesAsync(user);
            await _userActivityRepository.AddAsync(userActivity);
            await _userActivityRepository.SaveChangesAsync();

            var userViewModel = _mapper.Map<UserLoginResponse>(user);

            var tokenResponse = _jwtAuthenticationManager.Authenticate(user, roles);
            userViewModel.AccessToken = tokenResponse.AccessToken;
            userViewModel.ExpiresIn = tokenResponse.ExpiresIn;
            userViewModel.RefreshToken = _jwtAuthenticationManager.GenerateRefreshToken(user.Id);

            return new SuccessResponse<UserLoginResponse>
            {
                Message = ResponseMessages.LoginSuccessResponse,
                Data = userViewModel
            };
        }


        /// <summary>
        /// This method is used to verify a user phone number by taking in the user id from the token entity and verifying the phone number
        /// against the phone number provided, also check if token is still valid
        /// After all checks, set token to invalid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="RestException"></exception>
        public async Task<SuccessResponse<object>> CompleteUserOnboarding(VerifyTokenDTO model)
        {
            var tokenEntity = await _tokenRepository.SingleOrDefaultNoTracking(x => x.OTPToken == model.Token);
            
            if (tokenEntity == null || tokenEntity.ExpiresAt < DateTime.UtcNow || !tokenEntity.IsValid)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.InvalidToken);

            var user = await _userRepository.SingleOrDefaultNoTracking(x => x.Id == tokenEntity.UserId);

            if (user.PhoneNumber != model.PhoneNumber)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.InvalidToken);

            user.PhoneNumberConfirmed = true;
            user.IsVerified = true;

            tokenEntity.IsValid = false;


            return new SuccessResponse<object>
            {
                Message = ResponseMessages.TokenVerificationSuccessResponse,
            };
        }

        public async Task<SuccessResponse<UserByIdDto>> GetUserById(Guid userId)
        {
            var user = await _userRepository.SingleOrDefaultNoTracking(x => x.Id == userId);

            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var userResponse = _mapper.Map<UserByIdDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            userResponse.Roles = new List<string>();

            foreach (var role in roles)
                userResponse.Roles.Add(role);

            return new SuccessResponse<UserByIdDto>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = userResponse
            };
        }

        public async Task<PagedResponse<IEnumerable<UserDto>>> GetUsers(ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var userQuery = _userRepository.GetUsersQuery(parameter.Search);
            var userResponses = userQuery.ProjectTo<UserDto>(_mapper.ConfigurationProvider);

            var users = await PagedList<UserDto>.CreateAsync(userResponses, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<UserDto>.CreateResourcePageUrl(parameter, name, users, urlHelper);

            var response = new PagedResponse<IEnumerable<UserDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = users,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        #region Private Functions
        private async Task AddUserToRoles(User user, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                if (!await _userManager.IsInRoleAsync(user, role))
                    await _userManager.AddToRoleAsync(user, role);
            }
        }
        #endregion

        public async void MakeRequest()
        {            
            // Request headers

            _httpCLient.DefaultRequestHeaders.CacheControl = CacheControlHeaderValue.Parse("no-cache");

            _httpCLient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "•••••");
            var uri = "https://wema-alatdev-apimgt.azure-api.net/alat-test/api/Shared/GetAllBanks";

            var response = await _httpCLient.GetAsync(uri);

        }
    }
}

