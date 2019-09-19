using System.Net;

namespace MainProject.Common.Models.Rest
{
    public class RestResponse
    {
        public bool IsSuccessStatusCode { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string Content { get; set; }

        public string ContentType { get; set; }

        public string ReasonPhrase { get; set; }
    }
}
