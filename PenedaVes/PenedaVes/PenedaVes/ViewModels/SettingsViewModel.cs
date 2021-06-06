using System.Collections.Generic;

namespace PenedaVes.ViewModels
{
    public class SettingsViewModel
    {
        public List<SpeciesBox> SpeciesBoxes { get; set; }
        public List<CameraBox> CameraBoxes { get; set; }

        public bool ReceiveSummary { get; set; }
        public string PhoneNumber { get; set; }
        public bool UseEmail { get; set; }
        public bool UseCellphone { get; set; }
    }

    public class SpeciesBox
    {
        public int SpeciesId { get; set; }
        public string CommonName { get; set; }
        public bool IsChecked { get; set; }
    }
    
    public class CameraBox
    {
        public int CameraId { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
}