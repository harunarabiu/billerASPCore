using System;
using System.ComponentModel.DataAnnotations;

namespace FirstApp.Models.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

    }

}
