using System.Collections.Generic;

namespace MainProject.Common.Data.Helpers.Models
{
    public class PagedResult<T>
    {
        public List<T> Data { get; set; }

        public long Total { get; set; }

        public int PageSize { get; set; }

        public int CurrentPage { get; set; }

        public int FirstPage => Total > 0 ? 1 : 0;

        public long LastPage => PageSize > 0 ? (Total / PageSize) + (Total % PageSize > 0 ? 1 : 0) : 0;

        public bool HasMorePages => LastPage > CurrentPage;

        public List<string> Errors { get; set; }
    }
}
