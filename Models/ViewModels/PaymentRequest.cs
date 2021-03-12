using System;
using System.ComponentModel.DataAnnotations;

namespace FirstApp.Models.ViewModels
{
    public class PaymentRequest
    {
    
        [Required]
        public string CustomerId { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public double Amount { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Service { get; set; }

        [Required]

        public string PaymentPlan { get; set; }
    }
}
