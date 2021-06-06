using System.Collections.Generic;

namespace PenedaVes.ViewModels
{
    public class RootViewModel
    {
        public List<UserBox> UserBoxesList;
    }

    public class UserBox
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public bool IsChecked { get; set; }
    }
}