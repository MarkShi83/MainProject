using System.Collections.Generic;
using System.Linq;

namespace MainProject.Common.Data.Helpers
{
    public abstract class Query
    {
        public const int MinimumPage = 0;

        public const int MinimumSize = 0;

        public const int MaximumSize = 100;

        public bool IsPopulated { get; set; }

        public List<string> Errors { get; set; } = new List<string>();

        public bool HasError => Errors?.Count > 0;

        public QueryAttribute QueryAttribute { get; set; }

        [QueryParameter(CanBeFiltered = false, IsMandatory = false)]
        public QueryParameterValue<int> Page { get; set; }

        [QueryParameter(CanBeFiltered = false, IsMandatory = false)]
        public QueryParameterValue<int> Size { get; set; }

        /// <summary>
        /// Translate page request to data range
        /// </summary>
        /// <returns>length and offset</returns>
        public (int, int) GetOffset()
        {
            var offset = (Page.Value - 1) * Size.Value;

            return (Size.Value, offset);
        }

        public bool Validate()
        {
            if (HasError)
            {
                return false;
            }

            var errors = ValidateValues().ToList();

            if (Page.Value < MinimumPage)
            {
                errors.Add("[page] must be greater than zero.");
            }

            if (Size.Value < MinimumSize || Size.Value > MaximumSize)
            {
                errors.Add("[size] value must be between 1 And 100.");
            }

            Errors.AddRange(errors);

            return errors.Count == 0;
        }

        protected virtual IEnumerable<string> ValidateValues()
        {
            yield break;
        }
    }
}
