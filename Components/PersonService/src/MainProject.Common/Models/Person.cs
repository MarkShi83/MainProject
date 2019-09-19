using System;

namespace MainProject.Common.Models
{
    public class Person
    {
        public long Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        public string First { get; set; }

        public string Last { get; set; }

        public int Age { get; set; }

        public Gender Gender { get; set; }
    }
}