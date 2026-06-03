using NewsPortal.Common.Models.Requests.Comment;
using NewsPortal.DAL.Repositories;
using NewsPOrtal.DAL.Mapping;
using NewsPOrtal.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPOrtal.DAL.Repositories
{
    public class CommentRepository 
    {
       
       

        public void AddComment(Comment comment)
        {
            using (var db = new NewsPortalContext())
            {
                db.Comments.Add(comment);
                db.SaveChanges();


            }
            

        }

        public Comment FindCommentById(int id)
        {
            using (var db=new NewsPortalContext())
            {
               return db.Comments.Find(id);
            }
        }

        public void RemoveComment(Comment comment)
        {
            using(var db=new NewsPortalContext())
            {
                db.Entry(comment).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }
    }
}
