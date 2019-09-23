using System;

namespace MainProject.Common.Data.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryParameterAttribute : Attribute
    {
        public bool CanBeFiltered { get; set; } = false;

        public bool CanBeSorted { get; set; } = false;

        public bool IsMandatory { get; set; } = true;

        public string ColumnName { get; set; }

        public string Prefix { get; set; }

        public string TablesIfValueProvided { get; set; }
    }
}
