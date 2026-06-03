using NewsPortal.Common.Models.Requests.Article;
using NewsPOrtal.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPOrtal.DAL.Mapping
{
    public static class MappArticle
    {
        public static Article Map(AddArticle addArticle)
        {
            
            if (addArticle == null)
            {
                return null;
            }
            else
            {
                return new Article()
                {
                    
                    Title= addArticle.Title,
                    Description= addArticle.Description,
                    Body=addArticle.Body,
                    CreatedOn= addArticle.CreatedOn,
                    CreatedBy=addArticle.CreatedBy,
                    Category_Id= addArticle.Category_Id,
                    ImageTitle= addArticle.ImageTitle,
                    ImagePath=addArticle.ImagePath,
                   
                };
            }
        }
    }
}
