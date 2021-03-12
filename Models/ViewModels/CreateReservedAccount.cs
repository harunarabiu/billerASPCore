using System;
namespace FirstApp.Models.ViewModels
{
    public class CreateReservedAccount
    {
        public string AccountReference {get; set;}
        public string AccountName {get; set;}
        public string CurrencyCode {get; set;}
        public string ContractCode {get; set;}
        public string CustomerEmail {get; set;}
        public string CustomerName { get; set; }
        public string CustomerBVN { get; set; }
    }
}
