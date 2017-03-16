namespace YaR.SpbEdu.Requests
{
    public class RequestResponse<T>
    {
        public bool Ok { get; set; }

        public string Description { get; set; }

        public T Result { get; set; }

        public long? ErrorCode { get; set; }
    }
}