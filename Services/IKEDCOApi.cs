using System;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refit;

namespace FirstApp.Services
{
    
    public interface IKEDCOApi
    {
        [Post("/v1/auth/login/")]
        Task<string> GetTokenAsync([Header("Authorization")] string authorization);

        //{merchantcode}/{CustomerIdentifier}/{accesstoken}?referencetype=accountnumber?postpaid={true}
        [Get("/Identification/{merchantId}/{customerId}/{accessToken}?referencetype={referenceType}&postpaid={isPostPaid}")]
        Task<IdentificationRequest> Identification (string customerId, string merchantId = "130", string accessToken = "AA4148ED5A2B3DBD362A61D734B63FEB", bool isPostPaid=false, string referenceType="meter");

        //{merchantId}/{Password}
        [Post("/auth/{merchantId}/{Password}")]
        Task<GetAuthToken> Auth(string merchantId="130", string Password="xyz");

        //{CustomerIdentifier}/{metertype}/{merchantname}/{transreference}/{amount}/{accesstoken}
        [Post("/Payment/{customerId}/{paymentPlan}/{merchantId}/{transactionRef}/{amount}/{accessToken}")]
        Task<PaymentRequest> Payment(string customerId, double amount, string transactionRef, string accessToken= "C7123300D5D241B577605FA3FD5B0939", string paymentPlan="prepaid", string merchantId="115");

        //{ResourceURL}/{merchantname}/{TransactionIdentifier}/{metertype}/{accesstoken}
        [Get("/Payment/{merchantId}/{transactionRef}/{customerId}/{accessToken}")]
        Task<PaymentRequest> PaymentVerification(string customerId, string transactionRef, string accessToken= "AA4148ED5A2B3DBD362A61D734B63FEB", string merchantId= "130");

        //{ResourceURL}/{merchantname}/{metertype}/{TransactionIdentifier}/{accesstoken}/reversal
        [Post("/Payment/{merchantId}/{paymentPlan}/{transactionRef}/{accessToken}/reversal")]
        Task<PaymentReversal> PaymentReversal(string transactionRef, string accessToken= "AA4148ED5A2B3DBD362A61D734B63FEB", string merchantId = "130", string paymentPlan="postpaid");
    }




    public class GetAuthToken
    {
        [JsonProperty(PropertyName = "dategenerated")]
        public string dategenerated { get; set; }
        [JsonProperty(PropertyName = "token")]
        public string token { get; set; }
        [JsonProperty(PropertyName = "merchentid")]
        public string merchentid { get; set; }
    }


    public class Customer
    {
        public string customerName { get; set; }
        public string address { get; set; }
        public string phoneNumber { get; set; }
        public string accountNumber { get; set; }
        public string meterNumber { get; set; }
        public string businessUnit { get; set; }
        public string customerArrears{ get; set; }
        public string minimumPurchase { get; set; }
        public string undertaking { get; set; }
        public string tariffCode { get; set; }
        public string tariff { get; set; }
    }


    public class PaymentRequest
    {
        public string merchantId { get; set; }
        public string paidamount { get; set; }
        public string recieptNumber { get; set; }
        public string transactionDate { get; set; }
        public string transactionReference { get; set; }
        public string transactionStatus { get; set; }
        public Customer customer { get; set; }
    }

    public class IdentificationRequest : Customer
    {

        public string lastTransactionDate { get; set; }

    }

    public class PaymentReversal
    {
        public string code { get; set; }
        public string message { get; set; }
    }

}
