using System;
using System.ComponentModel.DataAnnotations;

namespace FirstApp.Models.ViewModels
{
    public class CreatePaymentPlanVM
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public long ServicePlanId { get; set; }
        [Required]
        public string CommissionType { get; set; }
        [Required]
        public double Commission { get; set; }
        [Required]
        public double ConvienceFee { get; set; }
        [Required]
        public Boolean IsDefault { get; set; }
    }
}
