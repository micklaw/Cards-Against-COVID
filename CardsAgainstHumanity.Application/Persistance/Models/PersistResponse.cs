namespace CardsAgainstHumanity.Application.Persistance.Models
{
    public class PersistResponse
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public string ETag { get; set; }
    }

    public class PersistResponse<T> : PersistResponse
    {
        public T Result { get; set; }
    }
}
