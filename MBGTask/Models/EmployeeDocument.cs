using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MBGTask.Models;
using System.ComponentModel.DataAnnotations;

namespace MBGTask.Models
{
    public class EmployeeDocument
    {
        public Employee employee { get; set; }
        public Documents documents { get; set; }
        public IEnumerable<Documents> doclist { get; set; }
    }

    public class Documents
    {
        public Nullable<int> EmpDetId { get; set; }
        public Nullable<int> EmpId { get; set; }
        [Display(Name = "File Name")]
        [Required(ErrorMessage = "Please choose file name..")]
        public string FileName { get; set; }
        [Display(Name = "File")]
        [Required(ErrorMessage = "Please choose a file..")]
        public HttpPostedFileBase FilePath { get; set; }
        public string EncryptFilePath { get; set; }
        [Display(Name = "Uploaded Date")]
        public DateTime CreateDate { get; set; }
    }
}