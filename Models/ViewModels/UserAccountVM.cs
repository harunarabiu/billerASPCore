using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstApp.Models.ViewModels
{
    public class UserAccountVM
    {
        public ApplicationUser User {get; set;}
        public IEnumerable<CustomerPaymentPlan> PaymentPlans {get; set;}
        public IEnumerable<Wallet> Wallets {get; set;}
        public IEnumerable<ReservedAccount> BankAccounts {get; set;}
    }
}
