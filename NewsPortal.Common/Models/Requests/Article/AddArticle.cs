using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsPortal.Common.Models.Requests.Article
{
    public class AddArticle
    {
        //[Required(ErrorMessage = "Please enter Tittle")]
        public string Title { get; set; }

        //[Required(ErrorMessage = "Please enter short description")]
        public string Description { get; set; }
        //[Required(ErrorMessage = "Please enter body")]
        public string Body { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int Category_Id { get; set; }
       // [Required(ErrorMessage = "Please upload image")]
        public string ImageTitle { get; set; }
        public string ImagePath { get; set; }

    }
}
