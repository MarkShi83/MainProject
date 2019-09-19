using System;

namespace MainProject.Common.Models.Rest.PersonServiceApi
{
    public class PersonResponse
    {
        public long Id { get; set; }

        public string First { get; set; }

        public string Last { get; set; }

        public int Age { get; set; }

        public Gender Gender { get; set; }

        public int Count { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Updated { get; set; }
    }
}
