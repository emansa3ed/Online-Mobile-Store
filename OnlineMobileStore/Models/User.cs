
using OnlineMobileStore.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineMobileStore.Models;
using Microsoft.IdentityModel.Tokens;

namespace OnlineMobileStore.Models
{

    public class User
    {
        [Key]
        [MinLength(2, ErrorMessage = "User name should be at least 2 characters.")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }




        [MinLength(3, ErrorMessage = "User name should be at least 3 characters.")]
        [RegularExpression(@"^[A-Z][a-zA-Z]+$", ErrorMessage = "First Name must start with a capital letter and only contain letters  not white spaces.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }




        [MinLength(3, ErrorMessage = "Last Name should be at least 3 characters.")]
        [RegularExpression(@"^[A-Z][a-zA-Z]+$", ErrorMessage = "Last Name must start with a capital letter and only contain letters not white spaces.")]

        [Display(Name = "Last Name")]
        public string LastName { get; set; }





        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Password should be at least 5 characters.")]
        public string Password { get; set; }



        [NotMapped]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Password should be at least 5 characters.")]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }



        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        public string? Role { get; set; }

        [Phone]
        [MinLength(11, ErrorMessage = "Sorry you must enter a valid number of 11 digits")]
        [StringLength(11, ErrorMessage = "Sorry you must enter a valid number of 11 digits")]
       
        [Display(Name = "Phone Number")]
        public string PhoneNum { get; set; }

        public List<Order>? Orders { get; set; } = new();



        public static bool UserRegisteration(User customer, ApplicationDbContext dbContext)
        {

            if (dbContext == null) 
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            customer.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(customer.Password);
            User CustomerName = dbContext.User.SingleOrDefault(mem => mem.UserName == customer.UserName);

            User existingUser = dbContext.User.SingleOrDefault(mem => mem.Email == customer.Email.ToLower());
            if (CustomerName != null || existingUser != null)
            {

                return false;
            }
            else
            {
                if (dbContext == null) 
                {
                    throw new ArgumentNullException(nameof(dbContext));
                }
                customer.Email = customer.Email.ToLower();
                customer.Role = Roles.ROLEUSER;
                dbContext.User.Add(customer);
                dbContext.SaveChanges();
                return true;
            }
        }


        public static string UserLogin(User loginCustomer, ApplicationDbContext dbContext)
        {

            User user = dbContext.User.SingleOrDefault(mem => mem.UserName == loginCustomer.UserName);


            if (user != null && BCrypt.Net.BCrypt.EnhancedVerify(loginCustomer.Password, user.Password) && user.Role == "User")
            {
                return "User";
            }
            else if (user != null && BCrypt.Net.BCrypt.EnhancedVerify(loginCustomer.Password, user.Password) && user.Role == "Admin")
            {
                return "Admin";
            }
            else if (user != null && BCrypt.Net.BCrypt.EnhancedVerify(loginCustomer.Password, user.Password) && user.Role == "SuperAdmin")
            {
                return "SuperAdmin";
            }
            else
            {
                return "failed";
            }

        }
    }
}
