﻿using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using System.Net.Http.Headers;
using Domain.Common;
using Newtonsoft.Json;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace Application.Services.Implementations
{
	public class BankService: IBankService
	{
        private readonly WemaConfigSettings _wemaConfigSettings;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public BankService(IOptions<WemaConfigSettings> wemaConfigSettings, HttpClient httpClient, IMapper mapper)
        {
            _wemaConfigSettings = wemaConfigSettings.Value;
            _httpClient = httpClient;
            _mapper = mapper;
        }

        /// <summary>
        /// This method utilises the httpclient to make an api call to the given url.
        /// The api key is specified in the appsettings and it is expected to return an object with the array of bank names in it
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<SuccessResponse<IEnumerable<ExistingBankDto>>> GetExistingBanks()
        {
            _httpClient.DefaultRequestHeaders.CacheControl = CacheControlHeaderValue.Parse("no-cache");

            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _wemaConfigSettings.apiKey);
            var uri = "https://wema-alatdev-apimgt.azure-api.net/alat-test/api/Shared/GetAllBanks";

            var response = _httpClient.GetAsync(uri).Result;
            response.EnsureSuccessStatusCode();
            

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<CustomSuccessResponse<ExistingBankDto>>(json);

            if (apiResponse == null || apiResponse.Result == null || !apiResponse.Result.Any())
            {
                throw new Exception("No banks found in the API response.");
            }

            //return apiResponse.Result;

            return new SuccessResponse<IEnumerable<ExistingBankDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = apiResponse.Result
            };
        }

    }

    public class CustomSuccessResponse<T>
    {
        public List<T> Result { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> ErrorMessages { get; set; }
        public bool HasError { get; set; }
        public DateTime TimeGenerated { get; set; }
    }
}

