namespace MessengerClone.Service.Features.General.DTOs
{
    public class PaginatedResult<T> : DataResult<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public int TotalPages => TotalRecordsCount > 0 && PageSize > 0
            ? (int)Math.Ceiling((double)TotalRecordsCount / PageSize)
            : 0;
    }
}
