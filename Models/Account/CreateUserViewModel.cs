using System;
using System.ComponentModel.DataAnnotations;
namespace FirstApp.Models.Account
{
    public class CreateUserViewModel
    {
        
            [Required]
            public string Username { get; set; }

            [Display(Name = "First Name")]

            [Required]
            public string FirstName { get; set; }

            [Display(Name = "Middle Name")]
            public string MiddleName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            [Required]
            [Display(Name = "Company Name")]  
            public string CompanyName { get; set; }

            [Required]

            [EmailAddress, MaxLength(500)]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }

            [DataType(DataType.PhoneNumber)]
            [StringLength(100, MinimumLength = 10, ErrorMessage = "Phone number invalid")]
            public string Phone { get; set; }

            [Required]
            public string AccountType { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            [Required]
            [Compare("Password", ErrorMessage ="Passwords must match")]
            [Display(Name = "Confirm Password")]
            [DataType(DataType.Password)]
            public string ConfirmPassword { get; set; }

           
       
    }
}
