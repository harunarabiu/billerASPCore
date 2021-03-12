using System;
using System.Threading.Tasks;
using FirstApp.Models.ViewModels;
using Newtonsoft.Json;
using Refit;

namespace FirstApp.Services
{
    public interface IPayant
    {
        //{merchantId}/{Password}
        [Put("/payment")]
        Task<PaymentVerify> verify([Body] GladePaymentVerificationRequest request, [Header("key")] string key = "tLwqLw5c9ocSkqi6fDZmFYQCQUhtqWxtXwb", [Header("mid")] string mid = "GP_VLX8t0rwdAXP9FElxtXdzlJeKymZFnkD");
    }

    public class PayantTransaction
    {
        public int status { get; set; }
        public string txnStatus { get; set; }
        public string txnRef { get; set; }
        public string chargedAmount { get; set; }
        public string customer_txnref { get; set; }
        public string payment_method { get; set; }
        public string currency { get; set; }
    }

    public class PayantPaymentVerify : Transaction
    {
        [JsonProperty(PropertyName = "card")]
        public TxnCardDetails cardDetails { get; set; }
    }



    public class PayantTxnCardDetails
    {
        [JsonProperty(PropertyName = "brand")]
        public string cardType { get; set; }
        [JsonProperty(PropertyName = "mask")]
        public string last4 { get; set; }
        public string bin { get; set; }
    }
}
