namespace MainProject.Common.Models.Rest.PersonServiceApi
{
    public class PersonRequest
    {
        public string First { get; set; }

        public string Last { get; set; }

        public int? Age { get; set; }

        public Gender? Gender { get; set; }

        public string GroupBy { get; set; }
    }
}
