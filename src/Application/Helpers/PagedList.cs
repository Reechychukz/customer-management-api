using System;
using Microsoft.EntityFrameworkCore;

namespace Application.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPrevious => (CurrentPage > 1);
        public bool HasNext => (CurrentPage < TotalPages); // linkedlist pattern

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        /// <summary>
        /// Create the pagged response of the query.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public static PagedList<T> Create(IQueryable<T> source, int pageNumber, int pageSize, string sort = null)
        {
            var count = source.Count();
            if (pageSize == 0)
            {
                pageSize = count;
            }

            if (!string.IsNullOrEmpty(sort))
            {
                var sortedData = SortHelper<T>.OrderByDynamic(source, sort);

                var items = sortedData.Skip(((pageNumber - 1) * pageSize)).Take(pageSize).ToList();
                return new PagedList<T>(items, count, pageNumber, pageSize);
            }
            else
            {
                var items = source.Skip(((pageNumber - 1) * pageSize)).Take(pageSize).ToList();
                return new PagedList<T>(items, count, pageNumber, pageSize);
            }
        }

        /// <summary>
        /// Async implementation to create source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public async static Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, string sort = null)
        {
            var count = source.Count();
            if (pageSize == 0)
            {
                pageSize = count;
            }

            if (!string.IsNullOrEmpty(sort))
            {
                var sortedData = SortHelper<T>.OrderByDynamic(source, sort);

                var items = await sortedData.Skip(((pageNumber - 1) * pageSize)).Take(pageSize).ToListAsync();
                return new PagedList<T>(items, count, pageNumber, pageSize);
            }
            else
            {
                var items = await source.Skip(((pageNumber - 1) * pageSize)).Take(pageSize).ToListAsync();
                return new PagedList<T>(items, count, pageNumber, pageSize);
            }
        }
    }

    public class ResourceParameter
    {
        private int _skip;

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? Sort { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        internal int Skip
        {
            get { return _skip; }
            private set
            {
                _skip = PageNumber > 1 ? (PageNumber * PageSize) / PageSize : 0;
            }
        }
        internal int Take { get { return PageSize; } }
    }

    /// <summary>
    /// Resource Uri Navigation type.
    /// </summary>
    public enum ResourceUriType
    {
        PreviousPage,
        NextPage,
        CurrentPage
    }
}

