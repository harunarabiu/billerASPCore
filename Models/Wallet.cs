using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstApp.Models
{
    public class Wallet : BaseModel
    {
        public Guid Id { get; set; }
        public double Balance { get; set; }
        public double BookBalance { get; set; }
        public string UserId { get; set; }
        //[ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set;}
        
    }

    
}
