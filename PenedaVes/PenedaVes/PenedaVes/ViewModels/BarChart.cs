using System.Collections.Generic;

namespace PenedaVes.ViewModels
{
    public class BarChart
    {
        public List<string> Labels { get; set; }
        public List<BarChartChild> Datasets { get; set; }
        public BarChart()
        {
            Labels = new List<string>();
            Datasets = new List<BarChartChild>();
        }
    }

    public class BarChartChild
    {
        public string Label { get; set; }
        public string BackgroundColor { get; set; }
        public string BorderColor { get; set; }
        public int BorderWidth { get; set; }
        public string HoverBackgroundColor{ get; set; }
        public string HoverBorderColor{ get; set; }
        public List<int> Data{ get; set; }
    }
}