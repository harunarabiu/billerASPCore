using System;
using System.ComponentModel.DataAnnotations;
namespace FirstApp.Models.Account
{
    public class LoginViewModel
    {
        public string Username { get; set; }

        [Required]
        [EmailAddress, MaxLength(500)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
