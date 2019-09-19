namespace MainProject.Common.Models.Rest
{
    public class RestOptions
    {
        public string PersonServiceApiUrl { get; set; }

        public int MaximumRetryOnGetRequests { get; set; } = 5;

        public int WaitTimeOutOnFailureGetRequestsInSecs { get; set; } = 5;
    }
}
