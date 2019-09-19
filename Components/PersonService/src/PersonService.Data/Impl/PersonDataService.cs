using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Dapper;

using MainProject.Common.Data;
using MainProject.Common.Models;
using MainProject.Common.Models.Rest.PersonServiceApi;

using Npgsql;

using NpgsqlTypes;

namespace PersonService.Data.Impl
{
    public class PersonDataService : IPersonDataService
    {
        private readonly ITransactionManager _transactionManager;

        public PersonDataService(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }

        public Task<PersonResponse> GetAsync(long id)
        {
            const string Query = @"
                                    SELECT  *
                                    FROM    person
                                    WHERE   id              = :id;";

            return _transactionManager
                .GetCurrentConnection()
                .QueryFirstOrDefaultAsync<PersonResponse>(
                    Query,
                    new { id });
        }

        public Task<IEnumerable<PersonResponse>> GetAsync(PersonRequest personRequest)
        {
            var query = @"SELECT  *
                          FROM    person ";

            if (!string.IsNullOrEmpty(personRequest.First) && !personRequest.First.Contains(";"))
            {
                query += $"WHERE first = '{personRequest.First}'";
            }

            if (!string.IsNullOrEmpty(personRequest.Last) && !personRequest.Last.Contains(";"))
            {
                if (!query.Contains("WHERE"))
                {
                    query += " WHERE ";
                }
                else
                {
                    query += " AND ";
                }

                query += $" last = '{personRequest.Last}'";
            }

            if (personRequest.Age != null)
            {
                if (!query.Contains("WHERE"))
                {
                    query += " WHERE ";
                }
                else
                {
                    query += " AND ";
                }

                query += $" age = {personRequest.Age}";
            }

            if (personRequest.Gender != null)
            {
                if (!query.Contains("WHERE"))
                {
                    query += " WHERE ";
                }
                else
                {
                    query += " AND ";
                }

                query += $" gender = {(int)personRequest.Gender}";
            }

            if (!string.IsNullOrEmpty(personRequest.GroupBy) && !personRequest.GroupBy.Contains(";"))
            {
                query = query.Replace("*", $"{personRequest.GroupBy}, COUNT(*) AS count");
                query += $" GROUP BY {personRequest.GroupBy} ";
            }

            query += ";";

            return _transactionManager
                .GetCurrentConnection()
                .QueryAsync<PersonResponse>(
                    query);
        }

        public Task<int> SaveAsync(
            Person person)
        {
            const string Query = @"
                                    INSERT  INTO person  (id, created, first, last, age, gender)
                                    VALUES               (:id, :create, :first, :last, :age, :gender)
                                    ON CONFLICT ON CONSTRAINT person_pkey
                                    DO UPDATE SET           first                  = EXCLUDED.first,
                                                            last                   = EXCLUDED.last,
                                                            age                    = EXCLUDED.age,
                                                            gender                 = EXCLUDED.gender,
                                                            updated                = :create;";

            return _transactionManager
                .GetCurrentConnection()
                .ExecuteAsync(
                    Query,
                    new
                    {
                        id = person.Id,
                        create = person.Created,
                        first = person.First,
                        last = person.Last,
                        age = person.Age,
                        gender = person.Gender
                    });
        }

        public async Task<int> SaveItemsAsync(IEnumerable<Person> persons)
        {
            var tableName = $"person_{Guid.NewGuid().ToString().Replace("-", string.Empty)}";
            var createTableCommand = $"CREATE TEMPORARY TABLE \"{tableName}\" (LIKE person) ON COMMIT DROP;";
            var copyFromCommand = $@"
                                    COPY ""{tableName}"" (id, created, first, last, age, gender)
                                    FROM STDIN (FORMAT BINARY);";

            var transferCommand = $@"
                                    INSERT  INTO    person  (id, created, first, last, age, gender)
                                    SELECT  id, created, first, last, age, gender
                                    FROM    ""{tableName}""
                                    ON CONFLICT ON CONSTRAINT person_pkey
                                    DO UPDATE SET           first                  = EXCLUDED.first,
                                                            last                   = EXCLUDED.last,
                                                            age                    = EXCLUDED.age,
                                                            gender                 = EXCLUDED.gender,
                                                            updated                = EXCLUDED.created;";

            var connection = (NpgsqlConnection)_transactionManager.GetCurrentConnection();
            await connection.ExecuteAsync(createTableCommand);
            using (var writer = connection.BeginBinaryImport(copyFromCommand))
            {
                foreach (var person in persons)
                {
                    writer.StartRow();
                    writer.Write(person.Id, NpgsqlDbType.Bigint);
                    writer.Write(person.Created, NpgsqlDbType.Timestamp);
                    writer.Write(person.First, NpgsqlDbType.Varchar);
                    writer.Write(person.Last, NpgsqlDbType.Varchar);
                    writer.Write(person.Age, NpgsqlDbType.Integer);
                    writer.Write((int)person.Gender, NpgsqlDbType.Integer);
                }

                writer.Complete();
            }

            return await connection.ExecuteAsync(transferCommand);
        }
    }
}