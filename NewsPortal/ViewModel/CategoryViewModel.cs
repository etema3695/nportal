using NewsPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewsPortal.ViewModel
{
    public class CategoryViewModel : BaseVieModel
    {
        public List<Article> Articles { get; set; }
        public List<CommentViewModel> Comments { get; set; }
       
    }

    public class CommentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your comment")]
        public string Body { get; set; }

        public int Article_Id { get; set; }
        public string ParentId { get; set; }
    }
}