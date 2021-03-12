using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FirstApp.Models.ViewModels
{
    public class WebhookBankTransfer
    {
        [JsonProperty(PropertyName = "transactionReference")]
        public string transactionReference { get; set; }
        public string paymentReference { get; set; }
        public string amountPaid { get; set; }
        public string totalPayable { get; set; }
        public string settlementAmount { get; set; }
        public string paidOn { get; set; }
        public string paymentStatus { get; set; }
        public string paymentDescription { get; set; }
        public string transactionHash { get; set; }
        public string currency { get; set; }
        public string paymentMethod { get; set; }
        public product product { get; set; }
        public cardDetails cardDetails { get; set; }
        public accountDetails accountDetails { get; set; }
        public IEnumerable<accountPayments> accountPayments { get; set; }
        public customer customer { get; set; }


    }

    public class product {
        public string type { get; set; }
        public string reference { get; set; }
    }
    public class cardDetails
    {
        public string cardType { get; set; }
        public string last4 { get; set; }
        public string bin { get; set; }
    }
    public class accountDetails {
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string bankCode { get; set; }
        public string amountPaid { get; set; }
    }
    public class accountPayments {
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string bankCode { get; set; }
        public string amountPaid { get; set; }
    } 
    public class customer
    {
        public string email { get; set; }
        public string name { get; set; }
    }

}
