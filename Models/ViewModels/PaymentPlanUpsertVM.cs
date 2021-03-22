using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstApp.Models.ViewModels
{
    public class PaymentPlanUpsertVM
    {
        [Required]
        public PaymentPlan Plan { get; set; }

        [Required]
        public IEnumerable<Service> Services { get; set; }
        [Required]
        public IEnumerable<ServicePlan> ServicePlans { get; set; }

    }
}
