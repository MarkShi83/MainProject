using System;

namespace MainProject.Common.Data.Helpers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class QueryAttribute : Attribute
    {
        public string TableName { get; set; }

        public string Prefix { get; set; }

        public string DefaultSortColumn { get; set; }

        public SortMethod DefaultSortMethod { get; set; }
    }
}
