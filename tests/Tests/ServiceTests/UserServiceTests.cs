using Moq;
using FluentAssertions;
using AutoMapper;
using Application.Services.Implementations;
using Application.Services.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Identity;
using Domain.Common;
using Domain.Enums;
using Application.Helpers;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRepository<UserActivity>> _mockUserActivityRepository;
    private readonly Mock<IRepository<Token>> _mockTokenRepository;
    private readonly Mock<IRepository<State>> _mockStateRepository;
    private readonly Mock<IRepository<LGA>> _mockLgaRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<RoleManager<Role>> _mockRoleManager;
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<IJwtAuthenticationManager> _mockJwtAuthenticationManager;
    private readonly Mock<HttpClient> _mockHttpClient;
    private readonly UserService _userService;
    private readonly Mock<IUrlHelper> _mockUrlHelper;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUserActivityRepository = new Mock<IRepository<UserActivity>>();
        _mockTokenRepository = new Mock<IRepository<Token>>();
        _mockStateRepository = new Mock<IRepository<State>>();
        _mockLgaRepository = new Mock<IRepository<LGA>>();
        _mockMapper = new Mock<IMapper>();
        _mockRoleManager = new Mock<RoleManager<Role>>(Mock.Of<IRoleStore<Role>>(), null, null, null, null);
        _mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        _mockJwtAuthenticationManager = new Mock<IJwtAuthenticationManager>();
        _mockHttpClient = new Mock<HttpClient>();
        _mockUrlHelper = new Mock<IUrlHelper>();

        _userService = new UserService(
            _mockUserRepository.Object,
            _mockUserActivityRepository.Object,
            _mockTokenRepository.Object,
            _mockMapper.Object,
            _mockRoleManager.Object,
            _mockUserManager.Object,
            _mockJwtAuthenticationManager.Object,
            _mockStateRepository.Object,
            _mockLgaRepository.Object,
            _mockHttpClient.Object
        );
    }

    [Fact]
    public async Task CreateUser_ShouldReturnSuccess_WhenUserIsCreated()
    {
        // Arrange
        var userSignupDto = new UserSignupDto
        {
            Email = "test@example.com",
            Password = "Password123",
            PhoneNumber = "1234567890",
            StateId = 1,
            LGAId = 1
        };

        var user = new User { Email = userSignupDto.Email, PhoneNumber = userSignupDto.PhoneNumber };
        var state = new State { Id = 1, Name = "Test State" };
        var lga = new LGA { Id = 1, Name = "Test LGA", StateId = 1 };

        _mockUserRepository.Setup(repo => repo.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(false);

        _mockStateRepository.Setup(repo => repo.FirstOrDefault(It.IsAny<System.Linq.Expressions.Expression<Func<State, bool>>>()))
            .ReturnsAsync(state);

        _mockLgaRepository.Setup(repo => repo.FirstOrDefault(It.IsAny<System.Linq.Expressions.Expression<Func<LGA, bool>>>()))
            .ReturnsAsync(lga);

        _mockMapper.Setup(m => m.Map<User>(userSignupDto)).Returns(user);

        _mockUserManager.Setup(manager => manager.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _mockRoleManager.Setup(manager => manager.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new Role { Name = ERole.USER.ToString() });

        _mockUserActivityRepository.Setup(repo => repo.AddAsync(It.IsAny<UserActivity>()))
            .Returns(Task.CompletedTask);

        _mockTokenRepository.Setup(repo => repo.AddAsync(It.IsAny<Token>()))
            .Returns(Task.CompletedTask);

        _mockUserActivityRepository.Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.FromResult(true));

        _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto
        {
            Email = userSignupDto.Email,
            PhoneNumber = userSignupDto.PhoneNumber,
            State = state.Name,
            LGA = lga.Name
        });

        // Act
        var result = await _userService.CreateUser(userSignupDto);

        // Assert
        result.Should().NotBeNull();
        result.Data.Email.Should().Be(userSignupDto.Email);
        result.Data.PhoneNumber.Should().Be(userSignupDto.PhoneNumber);
        result.Data.State.Should().Be(state.Name);
        result.Data.LGA.Should().Be(lga.Name);
        result.Message.Should().Be(ResponseMessages.CreationSuccessResponse);
    }

    [Fact]
    public async Task CreateUser_ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        var userSignupDto = new UserSignupDto
        {
            Email = "test@example.com",
            PhoneNumber = "1234567890",
            Password = "Password123",
            StateId = 1,
            LGAId = 1
        };

        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(true);

        var mockUserActivityRepository = new Mock<IRepository<UserActivity>>();
        var mockTokenRepository = new Mock<IRepository<Token>>();
        var mockStateRepository = new Mock<IRepository<State>>();
        var mockLGARepository = new Mock<IRepository<LGA>>();
        var mockMapper = new Mock<IMapper>();
        var mockRoleManager = new Mock<RoleManager<Role>>(
            Mock.Of<IRoleStore<Role>>(), null, null, null, null);
        var mockUserManager = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        var mockJwtAuthenticationManager = new Mock<IJwtAuthenticationManager>();
        var mockHttpClient = new Mock<HttpClient>();

        var userService = new UserService(
            mockUserRepository.Object,
            mockUserActivityRepository.Object,
            mockTokenRepository.Object,
            mockMapper.Object,
            mockRoleManager.Object,
            mockUserManager.Object,
            mockJwtAuthenticationManager.Object,
            mockStateRepository.Object,
            mockLGARepository.Object,
            mockHttpClient.Object
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RestException>(() => userService.CreateUser(userSignupDto));
        Assert.Equal(ResponseMessages.DuplicateEmail, exception.Message);
    }

    [Fact]
    public async Task CreateUser_ShouldThrowException_WhenPhoneNumberAlreadyExists()
    {
        // Arrange
        var userSignupDto = new UserSignupDto
        {
            Email = "test@example.com",
            PhoneNumber = "1234567890",
            Password = "Password123",
            StateId = 1,
            LGAId = 1
        };

        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.SetupSequence(repo => repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(false) // For email check
            .ReturnsAsync(true); // For phone number check

        var mockUserActivityRepository = new Mock<IRepository<UserActivity>>();
        var mockTokenRepository = new Mock<IRepository<Token>>();
        var mockStateRepository = new Mock<IRepository<State>>();
        var mockLGARepository = new Mock<IRepository<LGA>>();
        var mockMapper = new Mock<IMapper>();
        var mockRoleManager = new Mock<RoleManager<Role>>(
            Mock.Of<IRoleStore<Role>>(), null, null, null, null);
        var mockUserManager = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        var mockJwtAuthenticationManager = new Mock<IJwtAuthenticationManager>();
        var mockHttpClient = new Mock<HttpClient>();

        var userService = new UserService(
            mockUserRepository.Object,
            mockUserActivityRepository.Object,
            mockTokenRepository.Object,
            mockMapper.Object,
            mockRoleManager.Object,
            mockUserManager.Object,
            mockJwtAuthenticationManager.Object,
            mockStateRepository.Object,
            mockLGARepository.Object,
            mockHttpClient.Object
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RestException>(() => userService.CreateUser(userSignupDto));
        Assert.Equal(ResponseMessages.DuplicatePhoneNumber, exception.Message);
    }

    [Fact]
    public async Task CreateUser_ShouldGenerateOTPForPhoneNumberVerification()
    {
        // Arrange
        var userSignupDto = new UserSignupDto
        {
            Email = "test@example.com",
            PhoneNumber = "1234567890",
            Password = "Password123",
            StateId = 1,
            LGAId = 1
        };

        var user = new User
        {
            Email = userSignupDto.Email,
            PhoneNumber = userSignupDto.PhoneNumber
        };

        var state = new State { Id = 1, Name = "Test State" };
        var lga = new LGA { Id = 1, Name = "Test LGA", StateId = 1 };

        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(false);

        var mockUserActivityRepository = new Mock<IRepository<UserActivity>>();
        mockUserActivityRepository.Setup(repo => repo.AddAsync(It.IsAny<UserActivity>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var mockTokenRepository = new Mock<IRepository<Token>>();
        mockTokenRepository.Setup(repo => repo.AddAsync(It.IsAny<Token>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var mockStateRepository = new Mock<IRepository<State>>();
        mockStateRepository.Setup(repo => repo.FirstOrDefault(It.IsAny<Expression<Func<State, bool>>>()))
            .ReturnsAsync(state);

        var mockLGARepository = new Mock<IRepository<LGA>>();
        mockLGARepository.Setup(repo => repo.FirstOrDefault(It.IsAny<Expression<Func<LGA, bool>>>()))
            .ReturnsAsync(lga);

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.Map<User>(It.IsAny<UserSignupDto>()))
            .Returns(user);

        mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
            .Returns(new UserDto { State = state.Name, LGA = lga.Name });

        var mockRoleManager = new Mock<RoleManager<Role>>(
            Mock.Of<IRoleStore<Role>>(), null, null, null, null);
        mockRoleManager.Setup(r => r.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new Role { Name = ERole.USER.ToString() });

        var mockUserManager = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        mockUserManager.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var mockJwtAuthenticationManager = new Mock<IJwtAuthenticationManager>();

        var mockHttpClient = new Mock<HttpClient>();

        var userService = new UserService(
            mockUserRepository.Object,
            mockUserActivityRepository.Object,
            mockTokenRepository.Object,
            mockMapper.Object,
            mockRoleManager.Object,
            mockUserManager.Object,
            mockJwtAuthenticationManager.Object,
            mockStateRepository.Object,
            mockLGARepository.Object,
            mockHttpClient.Object
        );

        // Act
        var result = await userService.CreateUser(userSignupDto);

        // Assert
        mockTokenRepository.Verify(repo => repo.AddAsync(It.Is<Token>(token => token.TokenType == TokenTypeEnum.PHONENUMBER_COMFIRMATION && token.OTPToken.Length > 0)), Times.Once);
        Assert.True(result.Success);
    }

}


