using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MainProject.Common.Models;
using MainProject.Common.Models.Rest.PersonServiceApi;

namespace PersonService.Data
{
    public interface IPersonDataService
    {
        Task<PersonResponse> GetAsync(long id);

        //Task<IEnumerable<Person>> GetAsync(string firstName, string lastName, int? age, Gender? gender);

        Task<IEnumerable<PersonResponse>> GetAsync(PersonRequest personRequest);

        Task<int> SaveAsync(Person person);

        Task<int> SaveItemsAsync(IEnumerable<Person> persons);
    }
}