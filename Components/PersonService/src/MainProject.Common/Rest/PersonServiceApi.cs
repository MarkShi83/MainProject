using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MainProject.Common.Models;
using MainProject.Common.Models.Rest;
using MainProject.Common.Models.Rest.PersonServiceApi;

using Microsoft.Extensions.Options;

namespace MainProject.Common.Rest
{
    public class PersonServiceApi : IPersonServiceApi
    {
        private readonly IRestClient _restClient;

        private readonly RestOptions _restOptions;

        public PersonServiceApi(
            IRestClient restClient,
            IOptions<RestOptions> restOptions)
        {
            _restClient = restClient;
            _restOptions = restOptions.Value;
        }

        public Task<RestResponse<PersonResponse>> GetAsync(long id)
        {
            var restRequest = new RestRequest
                                  {
                                      RootUrl = _restOptions.PersonServiceApiUrl,
                                      Uri = $"persons/{id}",
                                      Payload = string.Empty
                                  };

            var cancellationTokenSource = new CancellationTokenSource();

            return _restClient.GetAsync<PersonResponse>(restRequest, cancellationTokenSource.Token);
        }

        public Task<RestResponse<PersonResponse[]>> GetAsync(PersonRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var queryStrings = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(request.First))
            {
                queryStrings.Add("first", request.First);
            }

            if (!string.IsNullOrEmpty(request.Last))
            {
                queryStrings.Add("last", request.Last);
            }

            if (request.Age != null)
            {
                queryStrings.Add("age", request.Age.ToString());
            }

            if (request.Gender != null)
            {
                queryStrings.Add("gender", ((int)request.Gender).ToString());
            }

            if (!string.IsNullOrEmpty(request.GroupBy))
            {
                queryStrings.Add("groupby", request.GroupBy);
            }

            var restRequest = new RestRequest
            {
                RootUrl = _restOptions.PersonServiceApiUrl,
                Uri = $"persons/",
                Payload = string.Empty,
                QueryStrings = queryStrings
            };

            var cancellationTokenSource = new CancellationTokenSource();

            return _restClient.GetAsync<PersonResponse[]>(restRequest, cancellationTokenSource.Token);
        }

        public Task<RestResponse<int>> PostAsync(Person person)
        {
            var restRequest = new RestRequest
                                  {
                                      RootUrl = _restOptions.PersonServiceApiUrl,
                                      Uri = $"persons",
                                      Payload = person.ToJsonString(),
                                      ContentType = RestRequest.JsonContentType
            };

            return _restClient.PostAsync<int>(restRequest);
        }
    }
}