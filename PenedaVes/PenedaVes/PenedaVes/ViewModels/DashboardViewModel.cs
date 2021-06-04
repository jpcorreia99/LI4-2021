using System.Collections.Generic;

namespace PenedaVes.ViewModels
{
    public class DashboardViewModel
    {
        public  List<CameraInfo> Cameras { get; set; }
        public string  BingApiKey { get; set; }
    }
    
    public class CameraInfo
    {
        public string Name { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public bool RestrictedZone { get; set; }
        public string PageUrl { get; set; }
        //Number of sightings in the last 7 days
        public int SightingCount { get; set; }
        
        public CameraInfo(string name, float latitude, float longitude, bool restrictedZone, string pageUrl, int sightingCount)
        {
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            RestrictedZone = restrictedZone;
            PageUrl = pageUrl;
            SightingCount = sightingCount;
        }
    }
}