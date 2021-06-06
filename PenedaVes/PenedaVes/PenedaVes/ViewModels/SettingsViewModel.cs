using System.Collections.Generic;
using System.ComponentModel;

namespace PenedaVes.ViewModels
{
    public class SettingsViewModel
    {
        public List<SpeciesBox> SpeciesBoxes { get; set; }
        public List<CameraBox> CameraBoxes { get; set; }

        [DisplayName("Receber Resumo?")]
        public bool ReceiveSummary { get; set; }
        
        [DisplayName("Número de telemóvel")]
        public string PhoneNumber { get; set; }
        
        [DisplayName("Usar email?")]
        public bool UseEmail { get; set; }
        
        [DisplayName("Usar Telemóvel?")]
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