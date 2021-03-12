using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FirstApp.Models
{
    public class Transaction : BaseModel
    {

        public Guid Id { get; set; }
        public string TransactionRef { get; set; }
        [Required]
        public double Amount { get; set;  }
        public double AmountCharged { get; set; }
        public double ServiceFee { get; set; }
        public double Commission { get; set; }
        //[ForeignKey(nameof(Service))]
        public long ServiceId { get; set; }
        public virtual Service Service { get; set; }
        public long PaymentPlanId { get; set; }
        //[ForeignKey("PaymentPlanId")]
        public virtual PaymentPlan PaymentPlan { get; set; }
        public string CustomerId { get; set; }
        public string Channel { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public string PaymentStatus { get; set; }
        public string ServiceRequestStatus { get; set; }
        public string TransactionToken { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public string RawData { get; set; }
    }


    

}
