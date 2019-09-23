namespace MainProject.Common.Data.Helpers
{
    public class QueryOperator
    {
        public enum OperatorType
        {
            Equal,
            NotEqual,
            Greater,
            GreaterAndEqual,
            Less,
            LessAndEqual,
            SimilarTo
        }

        public static QueryOperator[] Operators =>
            new[]
            {
                new QueryOperator { Type = OperatorType.Equal, Identifier = "==" },
                new QueryOperator { Type = OperatorType.NotEqual, Identifier = "!=" },
                new QueryOperator { Type = OperatorType.Greater, Identifier = "gt" },
                new QueryOperator { Type = OperatorType.GreaterAndEqual, Identifier = "ge" },
                new QueryOperator { Type = OperatorType.Less, Identifier = "lt" },
                new QueryOperator { Type = OperatorType.LessAndEqual, Identifier = "le" },
                new QueryOperator { Type = OperatorType.SimilarTo, Identifier = "~~" }
            };
        
        public OperatorType Type { get; set; }

        public string Identifier { get; set; }
    }
}
