namespace MessengerClone.API.Response
{
    public class ApiError
    {
        public string Code { get; private set; }
        public List<string> Details { get; private set; } = new List<string>();

        public ApiError(string code, params string[] details)
        {
            Code = code;
            Details.AddRange(details);
        }
    }
}