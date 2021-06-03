using System.Text.Json.Serialization;

namespace PenedaVesGenerator
{
    public class Species
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        
        [JsonPropertyName("commonName")] public string CommonName { get; set; }

        public Species(int id, string commonName)
        {
            Id = id;
            CommonName = commonName;
        }

        public override string ToString()
        {
            return "Id: " + Id + ", Name: " + CommonName;
        }
    }
}