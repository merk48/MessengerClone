namespace MessengerClone.Service.Features.General.DTOs
{
    public class DataResult<T>
    {
        /// <summary>
        /// All the data records.
        /// </summary>
        public IEnumerable<T> Data { get; set; } = null!;

        /// <summary>
        /// The number of data records with filter if exists.
        /// </summary>
        public int DataRecordsCount => Data.Count();

        /// <summary>
        /// The total number of records at Database all without filter
        /// </summary>
        public int TotalRecordsCount { get; set; }
    }
}
