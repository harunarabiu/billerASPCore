using System;
namespace FirstApp.Models.ViewModels
{
    public class TransactionResponse
    {
        public string TransactionRef { get; set; }
        public double Amount { get; set; }
        public double AmountCharged { get; set; }
        public double ServiceFee { get; set; }
        public string ServiceName { get; set; }
        public string PaymentType { get; set; }
        public string PaymentPlan { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}
