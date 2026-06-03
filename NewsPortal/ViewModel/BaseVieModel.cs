using NewsPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsPortal.ViewModel
{
    public class BaseVieModel : ViewModel.CommentViewModel
    {
        public List<CategoryItem> Categories { get; set; }
       
    }

    public class CategoryItem
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Parent_Id { get; set; }
    }

    public class ArticleItem
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Body { get; set; }

        public string CreatedBy { get; set; }


        public DateTime? CreatedOn { get; set; }

        public int Category_Id { get; set; }




        public string ImageTitle { get; set; }

        public string ImagePath { get; set; }
    }

}