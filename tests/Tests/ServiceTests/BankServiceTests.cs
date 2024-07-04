using Moq;
using Moq.Protected;
using Xunit;
using Application.Services.Implementations;
using Application.DTOs;
using Application.Helpers;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using AutoMapper;
using Domain.Common;

public class BankServiceTests
{
    private readonly Mock<IOptions<WemaConfigSettings>> _mockOptions;
    private readonly Mock<HttpMessageHandler> _mockMessageHandler;
    private readonly Mock<IMapper> _mockMapper;
    private readonly BankService _bankService;

    public BankServiceTests()
    {
        _mockOptions = new Mock<IOptions<WemaConfigSettings>>();
        _mockMessageHandler = new Mock<HttpMessageHandler>();
        _mockMapper = new Mock<IMapper>();

        // Set up WemaConfigSettings
        _mockOptions.Setup(opt => opt.Value).Returns(new WemaConfigSettings { apiKey = "test-api-key" });

        // Setup HttpMessageHandler to mock HttpClient
        _mockMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<System.Threading.CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
            })
            .Verifiable();

        var httpClient = new HttpClient(_mockMessageHandler.Object);

        // Create an instance of BankService
        _bankService = new BankService(
            _mockOptions.Object,
            httpClient,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task GetExistingBanks_ShouldReturnBanks_WhenApiCallIsSuccessful()
    {
        // Arrange
        var existingBanks = new List<ExistingBankDto>
        {
            new ExistingBankDto { bankName = "Bank A" },
            new ExistingBankDto { bankName = "Bank B" }
        };

        var apiResponse = new CustomSuccessResponse<ExistingBankDto>
        {
            Result = existingBanks,
            ErrorMessage = null,
            ErrorMessages = null,
            HasError = false,
            TimeGenerated = DateTime.UtcNow
        };

        _mockMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri.ToString() == "https://wema-alatdev-apimgt.azure-api.net/alat-test/api/Shared/GetAllBanks"
                ),
                ItExpr.IsAny<System.Threading.CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(apiResponse))
            });

        // Act
        var result = await _bankService.GetExistingBanks();

        // Assert
        Assert.Equal(ResponseMessages.RetrievalSuccessResponse, result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(existingBanks.Count, result.Data.Count());
        Assert.Contains(result.Data, bank => bank.bankName == "Bank A");
        Assert.Contains(result.Data, bank => bank.bankName == "Bank B");
    }

    [Fact]
    public async Task GetExistingBanks_ShouldThrowException_WhenNoBanksFound()
    {
        // Arrange
        var apiResponse = new CustomSuccessResponse<ExistingBankDto>
        {
            Result = new List<ExistingBankDto>(),
            ErrorMessage = null,
            ErrorMessages = null,
            HasError = false,
            TimeGenerated = DateTime.UtcNow
        };

        _mockMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri.ToString() == "https://wema-alatdev-apimgt.azure-api.net/alat-test/api/Shared/GetAllBanks"
                ),
                ItExpr.IsAny<System.Threading.CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(apiResponse))
            });

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bankService.GetExistingBanks());
    }

    [Fact]
    public async Task GetExistingBanks_ShouldThrowHttpRequestException_WhenApiCallFails()
    {
        // Arrange
        _mockMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<System.Threading.CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("API request failed"));

        // Act & Assert
        var aggregateException = await Assert.ThrowsAsync<AggregateException>(async () =>
        {
            await _bankService.GetExistingBanks();
        });

        var httpRequestException = aggregateException.GetBaseException() as HttpRequestException;
        Assert.NotNull(httpRequestException);
        Assert.Equal("API request failed", httpRequestException.Message);
    }
}
