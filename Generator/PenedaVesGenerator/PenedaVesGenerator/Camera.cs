using System.Text.Json.Serialization;

namespace PenedaVesGenerator
{
    public class Camera
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        
        [JsonPropertyName("name")] public string Name { get; set; }
        
        [JsonPropertyName("restrictedZone")] public bool RestrictedZone { get; set; }

        public Camera(int id, string name, bool restrictedZone)
        {
            Id = id;
            Name = name;
            RestrictedZone = restrictedZone;
        }

        public override string ToString()
        {
            string res = "Id: " + Id + ", Name: " + Name;

            if (RestrictedZone)
            {
                res += " !";
            }

            return res;
        }
    }
}