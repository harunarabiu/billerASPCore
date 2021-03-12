using System;
using System.ComponentModel.DataAnnotations;

namespace FirstApp.Models.ViewModels
{
    public class GladePaymentVerificationRequest
    {
        public string txnRef { get; set; }
        [Required]
        public string action { get; set; }
    }
}
