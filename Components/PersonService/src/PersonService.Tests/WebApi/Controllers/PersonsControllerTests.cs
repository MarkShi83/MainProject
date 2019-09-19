using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using MainProject.Common.Models;
using MainProject.Common.Models.Rest.PersonServiceApi;

using Microsoft.AspNetCore.Mvc;

using NSubstitute;

using PersonService.Data;
using PersonService.Tests.Data;
using PersonService.WebApi.Controllers;

using Xunit;

namespace PersonService.Tests.WebApi.Controllers
{
    public class PersonsControllerTests
    {
        private readonly IPersonDataService _dataService;

        private readonly IFixture _fixture;

        private readonly PersonsController _personsController;

        private readonly PersonResponse _personResponse;

        public PersonsControllerTests()
        {
            _fixture = new Fixture();

            var transactionManager = new MockTransactionManager();
            _dataService = Substitute.For<IPersonDataService>();

            _personsController = new PersonsController(transactionManager, _dataService);

            _personResponse = _fixture.Create<PersonResponse>();
            _personResponse.Count = 0;
        }

        [Fact]
        public async Task WhenGetPersonExistedReturnCorrectValue()
        {
            _dataService
                .GetAsync(_personResponse.Id)
                .Returns(_personResponse);

            var actual = await _personsController.GetAsync(_personResponse.Id);

            actual
                .Value
                .Should()
                .BeEquivalentTo(_personResponse);
        }

        [Fact]
        public async Task WhenPersonalInformationDoesNotExistThenNotFoundIsReturned()
        {
            _dataService
                .GetAsync(Arg.Any<long>())
                .Returns((PersonResponse)null);

            var actual = await _personsController.GetAsync(_personResponse.Id);

            actual
                .Result
                .Should()
                .BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task WhenPostPersonItShouldReturn1()
        {
            var person = _fixture.Create<Person>();
            _dataService
                .SaveAsync(person)
                .Returns(1);

            var actual = await _personsController.PostAsync(person);

            actual
                .Value
                .Should()
                .Be(1);
        }

        [Fact]
        public async Task WhenBulkPostPersonItShouldReturnTotalNumberOfPerson()
        {
            var persons = _fixture.Create<Person[]>();
            _dataService
                .SaveItemsAsync(persons)
                .Returns(persons.Length);

            var actual = await _personsController.PostBulkAsync(persons);

            actual
                .Value
                .Should()
                .Be(persons.Length);
        }
    }
}
