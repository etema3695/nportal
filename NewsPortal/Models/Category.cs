using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewsPortal.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Display(Name="Category")]
        [Required(ErrorMessage = "Please enter Category name")]
        public string Code { get; set; }
        public string Parent_Id { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set;}

    }
}