using System.Linq;
using System.Threading.Tasks;

using MainProject.Common.Data;
using MainProject.Common.Models;
using MainProject.Common.Models.Rest.PersonServiceApi;

using Microsoft.AspNetCore.Mvc;

using PersonService.Data;

namespace PersonService.WebApi.Controllers
{
    [Route("/persons")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly ITransactionManager _transactionManager;

        private readonly IPersonDataService _dataService;

        public PersonsController(ITransactionManager transactionManager, IPersonDataService dataService)
        {
            _transactionManager = transactionManager;
            _dataService = dataService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PersonResponse>> GetAsync(long id)
        {
            PersonResponse result = null;

            await _transactionManager.DoInTransactionAsync(async () =>
            {
                result = await _dataService.GetAsync(id);
            });

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpGet]
        public async Task<ActionResult<PersonResponse[]>> GetAsync([FromQuery]PersonRequest request)
        {
            PersonResponse[] result = null;

            await _transactionManager.DoInTransactionAsync(async () =>
                {
                    result = (await _dataService.GetAsync(request)).ToArray();
                });

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        [HttpPost]
        public async Task<ActionResult<int>> PostAsync(Person person)
        {
            if (person == null)
            {
                return BadRequest();
            }

            var result = 0;

            await _transactionManager.DoInTransactionAsync(async () =>
            {
                result = await _dataService.SaveAsync(person);
            });

            return result;
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<int>> PostBulkAsync(Person[] persons)
        {
            if (persons == null)
            {
                return BadRequest();
            }

            var result = 0;

            await _transactionManager.DoInTransactionAsync(async () =>
                {
                    result = await _dataService.SaveItemsAsync(persons);
                });

            return result;
        }
    }
}
