namespace Application.Helpers
{
    public class Pagination
    {
        public string NextPage { get; set; }
        public string PreviousPage { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

