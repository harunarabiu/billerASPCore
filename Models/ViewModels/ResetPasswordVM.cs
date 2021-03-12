using System;
using System.ComponentModel.DataAnnotations;

namespace FirstApp.Models.ViewModels
{
    public class ResetPasswordVM
    {
        [Required]
        [DataType(DataType.Text)]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Comfirm Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords Doesn't Match.")]
        public string ConfirmPassword { get; set; }
    }
}
