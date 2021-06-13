using System;
using System.Text.Json.Serialization;

namespace PenedaVesGenerator
{
    public class Species
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("commonName")] public string CommonName { get; set; }
        [JsonPropertyName("isPredatory")] public bool IsPredatory{ get; set; }

        public Species(int id, string commonName, bool isPredatory)
        {
            Id = id;
            CommonName = commonName;
            IsPredatory = isPredatory;
        }

        public override string ToString()
        {
            String res = "Id: " + Id + ", Name: " + CommonName;
            
            if (IsPredatory)
            {
                res += " !";
            }

            return res;
        }
    }
}