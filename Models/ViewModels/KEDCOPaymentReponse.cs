using System;
namespace FirstApp.Models.ViewModels
{
    public class KEDCOPaymentReponse : TransactionResponse
    {
        public string RechargeToken { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string AccountNumber { get; set; }
        public string MeterNumber { get; set; }
        public string BusinessUnit { get; set; }
        public string CustomerArrears { get; set; }
        public string MinimumPurchase { get; set; }
        public string TariffCode { get; set; }
        public string Tariff { get; set; }
    }
}
