using System;
using System.Collections.Generic;
using System.Transactions;

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

        //in order to deserialize this object from JSON these data types need to be primitive
        //I tried having a PaginationRequest object as the parameter but it didn't work when deserializing json (which makes sense).
        //Also the parameter names have to match what would be on the json, otherqise the compiler won't be able to figure it out.
        //This makes it so you have to break up pagenumber and pagesize when calling the constructor, but whatever.
        protected PaginationResponse(int pageNumber, int pageSize, int count, IEnumerable<T> listItems)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = count;
            ListItems = listItems;
        }
    }

}
