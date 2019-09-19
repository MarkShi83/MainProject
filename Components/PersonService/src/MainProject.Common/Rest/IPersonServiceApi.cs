using System.Threading.Tasks;

using MainProject.Common.Models;
using MainProject.Common.Models.Rest;
using MainProject.Common.Models.Rest.PersonServiceApi;

namespace MainProject.Common.Rest
{
    public interface IPersonServiceApi
    {
        Task<RestResponse<PersonResponse>> GetAsync(long id);

        Task<RestResponse<PersonResponse[]>> GetAsync(PersonRequest request);

        Task<RestResponse<int>> PostAsync(Person person);
    }
}
