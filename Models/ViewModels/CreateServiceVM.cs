using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstApp.Models.ViewModels
{
    public class CreateServiceVM
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public long ServiceType { get; set; }
       
    }
}
