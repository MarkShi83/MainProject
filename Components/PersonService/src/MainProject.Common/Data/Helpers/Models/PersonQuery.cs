using MainProject.Common.Models;

namespace MainProject.Common.Data.Helpers.Models
{
    [Query(
        TableName = "person",
        Prefix = "per",
        DefaultSortMethod = SortMethod.Desc,
        DefaultSortColumn = "id")]
    public class PersonQuery : Query
    {
        [QueryParameter(CanBeFiltered = true, IsMandatory = false, CanBeSorted = true)]
        public QueryParameterValue<long> Id { get; set; }

        [QueryParameter(CanBeFiltered = true, IsMandatory = false, CanBeSorted = true)]
        public QueryParameterValue<string> First { get; set; }

        [QueryParameter(CanBeFiltered = true, IsMandatory = false, CanBeSorted = true)]
        public QueryParameterValue<string> Last { get; set; }

        [QueryParameter(CanBeFiltered = true, IsMandatory = false, CanBeSorted = true)]
        public QueryParameterValue<int> Age { get; set; }

        [QueryParameter(CanBeFiltered = true, IsMandatory = false, CanBeSorted = true)]
        public QueryParameterValue<Gender> Gender { get; set; }
    }
}
