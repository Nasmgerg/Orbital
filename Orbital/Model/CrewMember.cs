namespace Orbital.Model
{
    public class CrewMember
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public int Shift { get; set; }
    }
}
