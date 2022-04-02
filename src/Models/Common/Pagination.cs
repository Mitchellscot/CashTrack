using System;
using System.Collections.Generic;

namespace CashTrack.Models.Common
{
    public abstract class PaginationRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string Query { get; set; } = null;
    }
    public abstract class PaginationResponse<T> where T : class
    {
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalCount / PageSize);
        public IEnumerable<T> ListItems { get; private set; }

        protected PaginationResponse(int pageNumber, int pageSize, int count, IEnumerable<T> listItems)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = count;
            ListItems = listItems;
        }
    }
}
