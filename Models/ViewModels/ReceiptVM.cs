using System;
using System.ComponentModel.DataAnnotations;

namespace FirstApp.Models.ViewModels
{
    public class ReceiptVM
    {
        [Required]
        public string TransactionRef { get; set; }
        [Required]
        public double Amount { get; set; }
        public double AmountCharged { get; set; }
        public double ServiceFee { get; set; }
        public double Commission { get; set; }
        public string Service { get; set; }
        public string ServiceType { get; set; }
        public string PaymentPlan { get; set; }
        public string Customer { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class KEDCOReceiptVM : ReceiptVM
    {
        public string RechargeToken { get; set; }
        public string Address { get; set; }
        public string CustomerID { get; set; }
        public string Tariff { get; set; }
        public string Arrears { get; set; }
        public string TariffClass { get; set; }
        public string AccountNumber { get; set; }
        public string Region { get; set; }
    }
}
