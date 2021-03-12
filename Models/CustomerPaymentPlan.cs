using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstApp.Models
{
    public class CustomerPaymentPlan: BaseModel
    {
        
            public long Id { get; set; }
            public string UserId { get; set; }
            [ForeignKey("UserId")]
            public virtual ApplicationUser User { get; set; }
            public long PaymentPlanId { get; set; }
            [ForeignKey("PaymentPlanId")]
            public virtual PaymentPlan PaymentPlan { get; set; }
   
    }
}
