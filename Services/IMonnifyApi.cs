using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FirstApp.Models;
using FirstApp.Models.ViewModels;
using Refit;
using System.Collections;
using System.Collections.Generic;

namespace FirstApp.Services
{

    //[Headers("Authorization: Bearer")]
    public interface IMonnifyApi
    {

        //[Headers("Authorization: Basic ")]
        [Post("/v1/auth/login/")]
        Task<MonnifyAuthResponse> GetTokenAsync([Header("Authorization")] string authorization);

        [Post("/v1/bank-transfer/reserved-accounts")]
        Task<ReservedAccountResponse> CreateReservedAccountAsync([Header("Authorization")] string authorization, [Body] CreateReservedAccount AccountDetails);

        
        [Get("/v2/bank-transfer/reserved-accounts/{accountReference}")]
        Task<ReservedAccountResponse> GetReservedAccountDetails([Header("Authorization")] string authorization, string accountReference);

        [Get("/bank-transfer/reserved-accounts/{accountNumber}")]
        Task<ReservedAccountResponse> DeallocateReservedAccount([Header("Authorization")] string authorization, string accountNumber);

        [Get("/v2/transactions/{transactionReference}")]
        Task <TransactionResponse> GetTransaction([Header("Authorization")] string authorization, string transactionReference);

        //bank-transfer/reserved-accounts/transactions?accountReference={accountReference}&page=0&size=10}

        [Get("/v1/bank-transfer/reserved-accounts/transactions?accountReference={accountReference}&page=0&size=10}")]
        Task <IEnumerable<SingleTransaction>> GetAllTransactionsAsync([Header("Authorization")] string authorization, string accountReference );
    }


    public class ReservedAccountResponse : MonnifyReponse
    {

        public ReservedAccountBody responseBody { get; set; }

    }
    public class ReservedAccountBody
    {
        [JsonProperty(PropertyName = "contractCode")]
        public string contractCode { get; set; }

        [JsonProperty(PropertyName = "accountReference")]
        public string accountReference { get; set; }

        [JsonProperty(PropertyName = "accountName")]
        public string accountName { get; set; }

        [JsonProperty(PropertyName = "currencyCode")]
        public string currencyCode { get; set; }

        [JsonProperty(PropertyName = "customerEmail")]
        public string customerEmail { get; set; }

        [JsonProperty(PropertyName = "accountNumber")]
        public string accountNumber { get; set; }

        [JsonProperty(PropertyName = "bankName")]
        public string bankName { get; set; }

        [JsonProperty(PropertyName = "bankCode")]
        public string bankCode { get; set; }


        [JsonProperty(PropertyName = "reservationReference")]
        public string reservationReference { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string status { get; set; }

        [JsonProperty(PropertyName = "rcreatedOn")]
        public string createdOn { get; set; }
    }

    public class MonnifyReponse
    {
        [JsonProperty(PropertyName = "requestSuccessful")]
        public object requestSuccessful { get; set; }

        [JsonProperty(PropertyName = "responseMessage")]

        public string responseMessage { get; set; }

        [JsonProperty(PropertyName = "responseCode")]
        public string responseCode { get; set; }

    }

    public class MonnifyAuthResponse : MonnifyReponse
    {
        [JsonProperty(PropertyName = "responseBody")]

        public MonnifyAuthBody responseBody { get; set; }


        
    }

    public class MonnifyAuthBody
    {
        [JsonProperty(PropertyName = "accessToken")]

        public string accessToken { get; set; }
    }

    public class GetTransactions : MonnifyReponse
    {
        [JsonProperty(PropertyName = "responseBody")]

        public GetTransactionsBody responseBody { get; set; }
    }

    public class GetTransactionsBody
    {
        [JsonProperty(PropertyName = "content")]

        public IEnumerable<Transaction> Transactions { get; set; }
    }

    public class SingleTransaction
    {
        
        public TransactionCustomer customerDTO { get; set; } 
        public string providerAmount { get; set; }
        public string paymentMethod { get; set; }
        public string createdOn { get; set; }
        public string amount { get; set; }
        public string flagged { get; set; }
        public string providerCode { get; set; }
        public string fee { get; set; }
        public string currencyCode { get; set; }
        public string completedOn { get; set; }
        public string paymentDescription { get; set; }
        public string paymentStatus { get; set; }
        public string transactionReference { get; set; }
        public string paymentReference { get; set; }
        public string merchantCode { get; set; }
        public string merchantName { get; set; }
        public string payableAmount { get; set; }
        public string amountPaid { get; set; }
        public string completed { get; set; }

    }
     public class SingleTransactionCustomer
     {
            public string email { get; set; }
            public string name { get; set; }
            public string merchantCode { get; set; }
     }


    public class TransactionResponse : MonnifyReponse
    {
        [JsonProperty(PropertyName = "responseBody")]

        public TransactionResponseBody responseBody { get; set; }
    }



    public class TransactionResponseBody
    {

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
        public TransactionProduct product { get; set; }
        public TransactionCardDetails cardDetails { get; set; }
        public TransactionAccountDetails accountDetails { get; set; }
        public IEnumerable<TransactionAccountPayments> accountPayments { get; set; }
        public TransactionCustomer customer { get; set; }


    }

    public class TransactionProduct
    {
        public string type { get; set; }
        public string reference { get; set; }
    }
    public class TransactionCardDetails
    {
        public string cardType { get; set; }
        public string last4 { get; set; }
        public string bin { get; set; }
    }
    public class TransactionAccountDetails
    {
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string bankCode { get; set; }
        public string amountPaid { get; set; }
    }
    public class TransactionAccountPayments
    {
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string bankCode { get; set; }
        public string amountPaid { get; set; }
    }
    public class TransactionCustomer
    {
        public string email { get; set; }
        public string name { get; set; }
    }



}
