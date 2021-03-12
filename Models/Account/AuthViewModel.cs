using System;
using System.ComponentModel.DataAnnotations;
namespace FirstApp.Models.Account
{
    public class AuthViewModel
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
    }
}
