using System.Net;

namespace DataAccessLayer.Entities
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessages = new List<string>();
        }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> ErrorMessages { get; set; } = null!;
        public object Result { get; set; } = null!;
        public int TotalPages { get; set; }
    }
}
