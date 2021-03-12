using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstApp.Models
{
    public class ServicePlan : BaseModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public long ServiceId { get; set; }
        //[ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        public double Price { get; set; }
        public string CommissionType { get; set; }
        public double Commission { get; set; }
        public double ConvienceFee { get; set; }
        public Boolean IsDefault { get; set; }
    }
}
