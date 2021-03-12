using System;
using System.ComponentModel.DataAnnotations;

namespace FirstApp.Models.ViewModels
{
    public class VerifyBVNVM
    {
        [Required]
        [MaxLength(11)]
        [MinLength(11)]
        public string BVN { get; set; }
    }
}
