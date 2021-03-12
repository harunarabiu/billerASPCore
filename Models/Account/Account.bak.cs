using System;
using System.ComponentModel.DataAnnotations;
namespace FirstApp.Models
{
    public class AccountBak
    {
        public long Id { get; set; }
        
        [Required]
        public string Username { get; set; }

        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [StringLength(100, MinimumLength =10, ErrorMessage ="Phone number invalid")]
        public string Phone { get; set; }


        
        public string AccountType { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public Boolean Active { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Timestamp { get; set; }
    }
    //public class User
    //{
    //    public long Id { get; set; }
    //    public string Username { get; set; }
    //    public string FirstName { get; set; }
    //    public string MiddleName { get; set; }
    //    public string LastName { get; set; }
    //    public string Email { get; set; }
    //    public string Phone { get; set; }
    //    public string AccountType { get; set; }
    //    public string Password { get; set; }
    //    public Boolean Active { get; set; }
    //    public DateTime Timestamp { get; set; }
    //}


}
