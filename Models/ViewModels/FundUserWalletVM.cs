using System;
using System.ComponentModel.DataAnnotations;
namespace FirstApp.Models.ViewModels
{
    public class FundUserWalletVM
    {
        [Required]
        public string UserId { get; set; }

        public long WalletId { get; set; }
        [Required]
        public double Amount { get; set; }
    }
}
