using System;
using System.ComponentModel.DataAnnotations;
namespace FirstApp.Models.ViewModels
{
    public class AddUserPaymentPlanVM
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public long PaymentPlanId { get; set; }
    }
}
