using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MainProject.Common.Data;
using MainProject.Common.Data.Helpers;
using MainProject.Common.Data.Helpers.Models;
using MainProject.Common.Models;
using MainProject.Common.Models.Rest.PersonServiceApi;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;

using PersonService.Data;

namespace PersonService.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly ITransactionManager _transactionManager;

        private readonly IPersonDataService _dataService;

        private readonly IQueryParameterParser _queryParameterParser;

        private readonly IQueryRunner _queryRunner;


        public PersonsController(ITransactionManager transactionManager, IPersonDataService dataService, IQueryParameterParser queryParameterParser, IQueryRunner queryRunner)
        {
            _transactionManager = transactionManager;
            _dataService = dataService;
            _queryParameterParser = queryParameterParser;
            _queryRunner = queryRunner;
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
        public async Task<ActionResult<PersonResponse[]>> GetAsync([FromQuery]PersonRequest request, [FromQuery]string[] sort)
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

            Console.WriteLine("array size:" + sort.Length);

            return result;
        }

        [Route("v2")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<Person>>> GetAsync([FromQuery]string[] filter, [FromQuery]string[] sort)
        {
            Console.WriteLine("filter size: " + filter.Length);
            Console.WriteLine("sort size: " + sort.Length);

            var query = _queryParameterParser.Parse<PersonQuery>(filter, sort);

            if (query.HasError || !query.IsPopulated)
            {
                return BadRequest(new PagedResult<Person> { Errors = query.Errors });
            }

            var result = await _queryRunner.RunAsync<Person, PersonQuery>(query);

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
