using System;
using System.ComponentModel.DataAnnotations;
namespace FirstApp.Models
{
    public class ServiceViewModel
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public virtual ServiceType ServiceType { get; set; }
    }
}
