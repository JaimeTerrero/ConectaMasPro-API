namespace ConectaMasPro.Application.Common.Pagination
{
    public class PagedResponse<T>
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }

        public bool HasPreviousPage => Page > 1;

        public bool HasNextPage => Page < TotalPages;

        public T Data { get; set; } = default!;

        public static PagedResponse<T> Create(
            T data,
            int page,
            int pageSize,
            int totalRecords)
        {
            return new PagedResponse<T>
            {
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                Data = data
            };
        }
    }
}
