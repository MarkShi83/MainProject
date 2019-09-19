using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MainProject.Common.Models.Rest;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MainProject.Common.Rest
{
    public class RestClient : IRestClient
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<RestClient> _logger;

        private readonly RestOptions _restOptions;

        public RestClient(ILogger<RestClient> logger, HttpClient httpClient, IOptions<RestOptions> restOptions)
        {
            _logger = logger;
            _httpClient = httpClient;
            _restOptions = restOptions.Value;
        }

        public async Task<RestResponse<T>> GetAsync<T>(RestRequest restRequest, CancellationToken cancellationToken)
        {
            var retries = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (var httpRequestMessage = CreateRequest(restRequest, HttpMethod.Get))
                    {
                        using (var response = await _httpClient.SendAsync(httpRequestMessage, cancellationToken))
                        {
                            var result = await PostProcessResultAsync<T>(response);

                            if (result.IsSuccessStatusCode)
                            {
                                return result;
                            }

                            switch (result.StatusCode)
                            {
                                case HttpStatusCode.RequestTimeout:
                                case HttpStatusCode.GatewayTimeout:

                                    throw new InvalidOperationException("Should retry!");

                                default:
                                    return result;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    retries++;

                    _logger.LogError(e.ToString());

                    if (retries > _restOptions.MaximumRetryOnGetRequests)
                    {
                        throw;
                    }

                    cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(_restOptions.WaitTimeOutOnFailureGetRequestsInSecs));
                }
            }

            return new RestResponse<T>
            {
                IsSuccessStatusCode = false,
                ReasonPhrase = "App_Root_Token_Is_Cancelled",
                StatusCode = HttpStatusCode.InternalServerError
            };
        }

        public async Task<RestResponse<T>> PostAsync<T>(RestRequest restRequest)
        {
            try
            {
                using (var httpRequestMessage = CreateRequest(restRequest, HttpMethod.Post))
                {
                    httpRequestMessage.Content = new StringContent(
                        restRequest.Payload,
                        Encoding.UTF8,
                        restRequest.ContentType);

                    using (var response = await _httpClient.SendAsync(httpRequestMessage))
                    {
                        return await PostProcessResultAsync<T>(response);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }

        public async Task<RestResponse> PutAsync(RestRequest restRequest)
        {
            try
            {
                using (var httpRequestMessage = CreateRequest(restRequest, HttpMethod.Put))
                {
                    if (!string.IsNullOrEmpty(restRequest.Payload))
                    {
                        httpRequestMessage.Content = new StringContent(
                            restRequest.Payload,
                            Encoding.UTF8,
                            restRequest.ContentType);
                    }

                    using (var response = await _httpClient.SendAsync(httpRequestMessage))
                    {
                        var result = new RestResponse
                        {
                            IsSuccessStatusCode = response.IsSuccessStatusCode,
                            ReasonPhrase = response.ReasonPhrase,
                            StatusCode = response.StatusCode
                        };

                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }

        private static async Task<RestResponse<T>> PostProcessResultAsync<T>(HttpResponseMessage response)
        {
            var result = new RestResponse<T>
            {
                IsSuccessStatusCode = response.IsSuccessStatusCode,
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode
            };

            if (!response.IsSuccessStatusCode || response.Content == null)
            {
                return result;
            }

            using (var data = response.Content)
            {
                if (data == null)
                {
                    return result;
                }

                if (data.Headers?.ContentType != null)
                {
                    result.ContentType = response.Content.Headers.ContentType.ToString();
                }

                var content = await data.ReadAsStringAsync();

                result.Content = result.ContentType.ToLowerInvariant().StartsWith(RestRequest.JsonContentType)
                                    ? content.FromJsonString<T>()
                                    : content.FromXmlString<T>();

                return result;
            }
        }

        private static string BuildUrl(RestRequest restRequest)
        {
            if (string.IsNullOrEmpty(restRequest.RootUrl))
            {
                throw new InvalidOperationException("Root Url is not defined.");
            }

            var result = new StringBuilder();

            result.Append(restRequest.RootUrl.TrimEnd('/'));

            if (!string.IsNullOrEmpty(restRequest.Uri))
            {
                result.Append($"/{restRequest.Uri.Trim('/')}");
            }

            var isFirst = true;

            foreach (var queryString in restRequest.QueryStrings)
            {
                if (string.IsNullOrEmpty(queryString.Value))
                {
                    continue;
                }

                result.Append(isFirst ? "?" : "&");
                isFirst = false;
                result.Append($"{queryString.Key}={queryString.Value}");
            }

            return result.ToString();
        }

        private static HttpRequestMessage CreateRequest(RestRequest restRequest, HttpMethod method)
        {
            var url = BuildUrl(restRequest);
            var httpRequestMessage = new HttpRequestMessage(method, url);

            if (restRequest.AuthenticationHeader != null)
            {
                httpRequestMessage.Headers.Authorization = restRequest.AuthenticationHeader;
            }

            httpRequestMessage.Headers.Add("Accept", restRequest.ContentType);

            if (restRequest.Headers != null && restRequest.Headers.Count > 0)
            {
                foreach (var header in restRequest.Headers)
                {
                    httpRequestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            return httpRequestMessage;
        }
    }
}
