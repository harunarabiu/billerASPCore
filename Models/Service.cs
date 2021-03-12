using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstApp.Models
{
    public class Service : BaseModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public virtual ServiceType ServiceType { get; set; }
       
    }
}
