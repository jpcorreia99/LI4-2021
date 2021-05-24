using System;
using System.ComponentModel.DataAnnotations;

namespace PenedaVes.ViewModels
{
    public class LoginViewModel
    {
        public String Username { get; set; }
        [DataType(DataType.Password)]
        public String Password { get; set; }
    }
}