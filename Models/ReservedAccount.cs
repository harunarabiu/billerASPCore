using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstApp.Models
{
    public class ReservedAccount:BaseModel
    {

        public Guid Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountCurrency { get; set; }
        public string AccountRef { get; set; }
        public string AccountReservationRef { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BVN { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public Guid WalletId { get; set; }
        [ForeignKey("WalletId")]
        public virtual Wallet Wallet { get; set; }

    }
}
