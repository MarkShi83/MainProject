using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace MainProject.Common.Models.Rest
{
    public class RestRequest
    {
        public const string JsonContentType = "application/json";

        public const string XmlContentType = "application/xml";

        public const string FormUrlEncodedContentType = "application/x-www-form-urlencoded";

        public RestRequest()
        {
            QueryStrings = new Dictionary<string, string>();
            Headers = new Dictionary<string, string>();
        }

        public string RootUrl { get; set; }

        public string Uri { get; set; }

        public Dictionary<string, string> QueryStrings { get; set; }

        public string ContentType { get; set; } = XmlContentType;

        public string Payload { get; set; }

        public AuthenticationHeaderValue AuthenticationHeader { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public static AuthenticationHeaderValue CreateBasic(string userName, string password)
        {
            var value = Encoding.ASCII.GetBytes($"{userName}:{password}");
            var base64Token = Convert.ToBase64String(value);
            
            return new AuthenticationHeaderValue("Basic", base64Token);
        }
    }
}
