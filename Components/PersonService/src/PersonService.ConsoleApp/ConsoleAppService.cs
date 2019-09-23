using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MainProject.Common;
using MainProject.Common.Data.Helpers.Models;
using MainProject.Common.Models;
using MainProject.Common.Models.Rest.PersonServiceApi;
using MainProject.Common.Rest;

using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

using RestSharp;

using RestClient = RestSharp.RestClient;

namespace PersonService.ConsoleApp
{
    public class ConsoleAppService : BackgroundService
    {
        private readonly IApplicationLifetime _applicationLifetime;

        private readonly IPersonServiceApi _personServiceApi;

        public ConsoleAppService(IApplicationLifetime applicationLifetime, IPersonServiceApi personServiceApi)
        {
            _applicationLifetime = applicationLifetime;
            _personServiceApi = personServiceApi;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                PrepareData();

                Task1();

                Task2();

                Task3();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();

            _applicationLifetime.StopApplication();

            return Task.CompletedTask;
        }

        private static IEnumerable<Person> ReadPersonFromFile(string path)
        {
            using (var r = new StreamReader(path))
            {
                try
                {
                    var json = r.ReadToEnd();
                    var items = JsonConvert.DeserializeObject<Person[]>(json);
                    return items;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private void PrepareData()
        {
            // If there are huge amount of file need to be load to database 
            // I will change the way data got loaded. It can not throw the API anymore
            // All the data will save into Json format and upload into S3 bucket
            // There will be a new API endpoint which allow client to post the S3 file key
            // Once this API received the request it will send the request to a queue in AWS SQS
            // There will be a backend service which listen to the queue and will process the message
            // from the queue which will load the data from the file and save into database
            var persons = ReadPersonFromFile("C:/MainProject2/Data/example_data.json");

            Console.WriteLine("Preparing Data.....");
            foreach (var p in persons)
            {
                var response = _personServiceApi.PostAsync(p);
                if (!response.Result.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to add person id {p.Id} into database");
                }

                Console.WriteLine($"Person {p.Id} created successfully.");
            }

            Console.WriteLine("Finish Prepare Data.....");

            Console.WriteLine();
        }

        private void Task1()
        {
            Console.WriteLine("Task 1:");
            var person = _personServiceApi.GetAsync(42).Result.Content;

            Console.WriteLine("The users full name for id=42");

            Console.WriteLine("Result:");
            Console.WriteLine(person != null ? $"FullName: {person.First} {person.Last}" : "Not Found");

            Console.WriteLine();
        }

        private void Task2()
        {
            Console.WriteLine("Task 2:");
            Console.WriteLine("All the users first names (comma separated) who are 23");
            Console.WriteLine("Result:");

            var age = 23;
            var pagesize = 20;
            var pagenumber = 1;

            var client = new RestClient("http://localhost:5000");

            var request = new RestRequest("Persons/v2", Method.GET);
            request.AddQueryParameter("filter", $"[age]=={age}");
            request.AddQueryParameter("filter", $"[size]=={pagesize}");
            request.AddQueryParameter("filter", $"[page]=={pagenumber}");

            request.AddHeader("Content-Type", "application/json");

            var response = client.Execute(request);
            if (!response.IsSuccessful)
            {
                Console.WriteLine("Not Found");
                Console.WriteLine();
                return;
            }

            var result = response.Content.FromJsonString<PagedResult<Person>>();

            Console.WriteLine("FirstName: " + string.Join(",", result.Data.Select(x => x.First)));
            Console.WriteLine();
        }

        private void Task3()
        {
            Console.WriteLine("Task 3:");
            Console.WriteLine("The number of genders per Age, displayed from youngest to oldest");
            Console.WriteLine("Result:");

            var personRequest = new PersonRequest { GroupBy = "age, gender" };

            var results = _personServiceApi.GetAsync(personRequest).Result.Content;

            var output = string.Empty;
            foreach (var result in results.OrderBy(x => x.Age).ToList())
            {
                if (!output.Contains($"Age: {result.Age}"))
                {
                    if (!string.IsNullOrEmpty(output))
                    {
                        output += "\n";
                    }

                    output += $"Age: {result.Age}";
                }

                output += $" {result.Gender}: {result.Count}";
            }

            Console.WriteLine(output);
        }
    }
}