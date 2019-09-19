using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace PersonService.Tests
{
    public class HostTests
    {
        [Theory]
        [InlineData("schemaupdater")]
        [InlineData("webapi")]
        public void TestApps(string role)
        {
            Task.WaitAll(GetActions(role).ToArray());
        }

        private static IEnumerable<Task> GetActions(string role)
        {
            Environment.SetEnvironmentVariable("APPLICATIONNAME", role);

            yield return Task.Factory.StartNew(
                async () =>
                {
                    await Program.Main();
                });
            yield return Task.Factory.StartNew(
                async () =>
                {
                    Thread.Sleep(5000);
                    await Program.Host.StopAsync();
                });
        }
    }
}
