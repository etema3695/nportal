using NewsPortal.Common.Models.Requests.Comment;
using NewsPOrtal.DAL.Models;
using NewsPOrtal.DAL.Repositories;
using NewsPOrtal.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPortal.BLL.Services
{
    public class CommentService : BaseService
    {
       
        private readonly CommentRepository commentRepository=new CommentRepository();
        

        public void AddComment(CategoryViewModel request)
        {
           
            var commentModel = new Comment();
            commentModel.Id = request.Id;
            commentModel.Name = request.Name;
            commentModel.Body = request.Body;
            commentModel.Article_Id = request.Article_Id;

            commentRepository.AddComment(commentModel);


        }

        public Comment FindCommentById(int id)
        {
           return commentRepository.FindCommentById(id);
        }

        public void RemoveComment(Comment comment)
        {
            commentRepository.RemoveComment(comment);
        }
    }
}
