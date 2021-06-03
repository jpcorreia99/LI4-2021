using System.Text.Json.Serialization;

namespace PenedaVesGenerator
{
    public class Camera
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        
        [JsonPropertyName("name")] public string Name { get; set; }

        public Camera(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return "Id: " + Id + ", Name: " + Name;
        }
    }
}