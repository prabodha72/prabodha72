using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MBGTask.Models
{
    public class Employee
    {
        public Nullable<int> empid { get; set; }
        [Display(Name="Name")]
        [Required(ErrorMessage ="Please Enter Name..")]
        public string Name { get; set; }
        [Display(Name="Email")]
        [Required(ErrorMessage = "Please Enter Email..")]
        public string Email { get; set; }
        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Please Enter Designation..")]
        public string Designation { get; set; }
    }
}