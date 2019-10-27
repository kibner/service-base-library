using System.Collections.Generic;

namespace ServiceBaseLibrary
{
    public class Page<TEntity>
    {
        private const int DefaultPageNumber = 1;
        private const int DefaultPageSize = 10;

        public Page() : this(DefaultPageNumber, DefaultPageSize)
        {
        }

        public Page(int pageNumber, int pageSize) : this(pageNumber, pageSize, new List<TEntity>(), 0)
        {
        }

        public Page(int pageNumber, int pageSize, ICollection<TEntity> pageData, int totalRows)
        {
            PageNumber = pageNumber == 0 ? DefaultPageNumber : pageNumber;
            PageSize = pageSize == 0 ? DefaultPageSize : pageSize;
            PageData = pageData;
            TotalRows = totalRows;
        }

        public int NumberOfRecordsToSkip => (PageNumber - 1) * PageSize;

        public ICollection<TEntity> PageData { get; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRows { get; set; }
    }
}