namespace MainProject.Common.Data.Models
{
    public class DataOptions
    {
        public string Address { get; set; }

        public string Port { get; set; } = "5432";

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string JournalTable { get; set; } = "schema_version";

        public string ConnectionString => $"Server={Address};Port={Port};Database={Name};User Id={UserName};Password={Password};";
    }
}
