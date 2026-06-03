using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsPortal.ViewModel
{
    public class ArticleViewModel
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