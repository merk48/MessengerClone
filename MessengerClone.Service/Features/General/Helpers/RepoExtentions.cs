namespace MessengerClone.Service.Features.General.Extentions
{
    internal static class RepoExtentions
    {
        public static IQueryable<T> Pagination<T>(this IQueryable<T> source, int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
