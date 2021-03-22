using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstApp.Models.ViewModels
{
    public class ServicePlanVM
    {
        [Required]
        public ServicePlan Plan { get; set; }

        [Required]
        public IEnumerable<Service> Services { get; set; }

    }
}
