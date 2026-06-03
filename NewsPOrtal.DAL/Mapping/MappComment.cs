using NewsPortal.Common.Models.Requests.Comment;
using NewsPOrtal.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPOrtal.DAL.Mapping
{
    public static class MappComment
    {
        public static Comment Map(AddComment addComment)
        {
            if (addComment == null)
            {
                return null;
            }
            else
            {
                return new Comment()
                {
                    Name = addComment.Name,
                    Body = addComment.Body,
                    Article_Id = addComment.Article_Id
                };
            }
        }
    }
}
