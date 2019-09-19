using System;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using MainProject.Common.Models;

using PersonService.Data;

using Xunit;

namespace PersonService.Tests.Data.Impl
{
    public class PersonalInformationDataServiceTests : BaseDatabaseTests
    {
        private readonly IPersonDataService _personDataService;

        private readonly IFixture _fixture;

        private readonly DateTime _now;

        public PersonalInformationDataServiceTests(DatabaseFixture databaseFixture)
            : base(databaseFixture)
        {
            _personDataService = (IPersonDataService)CallScope.ServiceProvider.GetService(typeof(IPersonDataService));

            _now = new DateTime(2019, 09, 19);

            _fixture = new Fixture();
        }

        [Fact]
        public async Task WhenSaveRecordIfNotExistCreateNewElseUpdateExisted()
        {
            var person = _fixture.Create<Person>();
            person.Created = _now;
            await TransactionManager.DoInTransactionAsync(
                async () =>
                    {
                        var rows = await _personDataService.SaveAsync(person);

                        rows.Should().Be(1);

                        var actual = await _personDataService.GetAsync(person.Id);

                        actual.Id.Should().Be(person.Id);
                        actual.First.Should().Be(person.First);
                        actual.Last.Should().Be(person.Last);
                        actual.Age.Should().Be(person.Age);
                        actual.Gender.Should().Be(person.Gender);
                        actual.Created.Should().Be(_now);

                        var newAge = _fixture.Create<int>();
                        var date = _now.AddHours(1);

                        person.Age = newAge;
                        person.Created = date;
                        rows = await _personDataService.SaveAsync(person);

                        rows.Should().Be(1);

                        actual = await _personDataService.GetAsync(person.Id);

                        actual.Id.Should().Be(person.Id);
                        actual.First.Should().Be(person.First);
                        actual.Last.Should().Be(person.Last);
                        actual.Age.Should().Be(newAge);
                        actual.Gender.Should().Be(person.Gender);
                        actual.Created.Should().Be(_now);
                        actual.Updated.Should().Be(date);
                    });
        }
    }
}
