using System.Threading;
using System.Threading.Tasks;

using MainProject.Common.Models.Rest;

namespace MainProject.Common.Rest
{
    public interface IRestClient
    {
        Task<RestResponse<T>> GetAsync<T>(RestRequest restRequest, CancellationToken cancellationToken);

        Task<RestResponse<T>> PostAsync<T>(RestRequest restRequest);

        Task<RestResponse> PutAsync(RestRequest restRequest);
    }
}
