using System;
using System.ComponentModel.DataAnnotations;

namespace FirstApp.Models.ViewModels
{
    public class KEDCOTransactionVM: BaseModel
    {
        public Guid Id { get; set; }
        public string TransactionRef { get; set; }
        public string TransactionStatus { get; set; }
        public double Amount { get; set; }
        public double AmountCharged { get; set; }
        public double ServiceFee { get; set; }
        public double Commission { get; set; }
        public virtual Service Service { get; set; }
        public virtual PaymentPlan PaymentPlan { get; set; }
        public string Channel { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string TransactionToken { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string BusinessUnit { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public string MeterNumber { get; set; }
        public string RechargeToken { get; set; }
        public string TariffPlan { get; set; }
        public string TariffClass { get; set; }
        public string Arrears { get; set; }

    }
}
