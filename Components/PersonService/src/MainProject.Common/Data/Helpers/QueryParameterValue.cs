namespace MainProject.Common.Data.Helpers
{
    public class QueryParameterValue<T>
    {
        public SortMethod SortMethod { get; set; } = SortMethod.None;

        public bool ValueIsProvided { get; set; }

        public QueryOperator Operator { get; set; }

        public T Value { get; set; }
    }
}
