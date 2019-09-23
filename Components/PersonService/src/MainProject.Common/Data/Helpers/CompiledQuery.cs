using System.Collections.Generic;
using System.Dynamic;

namespace MainProject.Common.Data.Helpers
{
    public class CompiledQuery
    {
        public string Sql { get; set; }

        public string CountSql { get; set; }

        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        public Query Query { get; set; }

        public dynamic GetParameters()
        {
            IDictionary<string, object> result = new ExpandoObject();

            foreach (var item in Parameters)
            {
                result.Add(item);
            }

            return result;
        }
    }
}
