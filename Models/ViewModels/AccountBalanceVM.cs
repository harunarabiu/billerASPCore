using System;
namespace FirstApp.Models.ViewModels
{
    public class AccountBalanceVM
    {
        public Wallet Wallet { get; set; }
        public ReservedAccount BankAccount { get; set; }
    }
}
