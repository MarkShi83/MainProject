using System;

using FluentAssertions;

using PersonService.WebApi.Controllers;

using Xunit;

namespace PersonService.Tests.WebApi.Controllers
{
    public class HealthControllerTests
    {
        private readonly HealthController _healthController;

        public HealthControllerTests()
        {
            _healthController = new HealthController(() => new DateTime(2019, 08, 30));
        }

        [Fact]
        public void TestPing()
        {
            var result = _healthController.Ping();

            result.Should().NotBeEmpty();
        }
    }
}
