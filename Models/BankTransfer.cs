using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstApp.Models
{
    public class BankTransfer : BaseModel
    {
       
            public Guid Id { get; set; }
            public string TransactionReference { get; set; }
            public string PaymentReference { get; set; }
            public string AmountPaid { get; set; }
            public string TotalPayable { get; set; }
            public string SettlementAmount { get; set; }
            public string PaymentDate { get; set; }
            public string PaymentStatus { get; set; }
            public string PaymentDescription { get; set; }
            public string Currency { get; set; }
            public string PaymentMethod { get; set; }
            public string AccountPaymentDetails { get; set; }
            public string CardPaymentDetails { get; set; }
            public Guid UserId { get; set; }
            [ForeignKey("ServiceId")]
            public virtual ApplicationUser User { get; set; }


    }
}
