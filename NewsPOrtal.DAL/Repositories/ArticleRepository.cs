using NewsPortal.Common.Models.Requests.Article;
using NewsPortal.DAL.Repositories;
using NewsPOrtal.DAL.Mapping;
using NewsPOrtal.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Security.Principal;
using NewsPOrtal.DAL.ViewModels;
using NewsPortal.Common.Models;

namespace NewsPOrtal.DAL.Repositories
{
    public class ArticleRepository
    {

        public void AddArticle(AddArticle addArticle)
        {
          

                using (var db = new NewsPortalContext())
                {
                    
                    var entity= MappArticle.Map(addArticle);
                    db.Articles.Add(entity);
                    db.SaveChanges();
                    
                }
                
          

        }

        public Article FindArticleById(int? id)
        {
            using (var db = new NewsPortalContext())
            {

                AddArticle addArticle = new AddArticle();
                var article = MappArticle.Map(addArticle);

                article = db.Articles.Find(id);


                return article;
            }

        }

        public void RemoveArticle(Article article)
        {
            using (var db = new NewsPortalContext())
            {
                db.Entry(article).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        public void PublishArticle(Article article)
        {
            using (var db = new NewsPortalContext())
            {
                var entity = db.Articles.Find(article.Id);
                if (entity != null)
                {
                    entity.IsPublished = true;
                    db.SaveChanges();
                }
            }
        }

        public GenericResponse<List<Article>> GetAllArticles(IPrincipal User)
        {
            try
            {
                var result = new List<Article>();
                using (var db = new NewsPortalContext())
                {
                    var articles = db.Articles.Include(a => a.Category);
                    if (User.IsInRole("Journalist"))
                    {
                        result = articles.Where(c => c.CreatedBy == User.Identity.Name).ToList();
                    }
                    else
                    {
                        result = articles.ToList();
                    }
                }

                return new GenericResponse<List<Article>>()
                {
                    IsSuccessfull = true,
                    Result = result
                };
            }
            catch (Exception e)
            {
                return new GenericResponse<List<Article>>(e);
            }
        }

        public GenericResponse<Article> GetArticle(int? id)
        {
            try
            {
                var result = new Article();
                using (var db = new NewsPortalContext())
                {
                    var articles = db.Articles.Include(a => a.Category);
                    result = articles.Where(a => a.Id == id).FirstOrDefault();
                    

                }
                return new GenericResponse<Article>()
                {
                    IsSuccessfull = true,
                    Result = result
                };

            }
            catch (Exception e)
            {
                return new GenericResponse<Article>(e);
            }

        }

        public void EditArticle(Article article)
        {

            using (var db = new NewsPortalContext())
            {

                db.Entry(article).State = EntityState.Modified;

                db.SaveChanges();

            }
           
        }

        public List<Article>GetArticlesOfSameCategory(int? id)
        {
            using (var db=new NewsPortalContext())
            {
                var articles = db.Articles.Include(a => a.Category);
                return articles.Where(a => a.Category_Id == id).ToList();
            }
        }

        public List<Article>GetArticlesOfUser(int? id,IPrincipal User)
        {
            using (var db=new NewsPortalContext())
            {
                var articles = db.Articles.Include(a => a.Category);
                return articles.Where(a => a.Category_Id == id).Where(c => c.CreatedBy == User.Identity.Name).ToList();
            }
        }

        public List<Article> GetArticlesInNewsPortal()
        {
            using (var db = new NewsPortalContext())
            {
                return db.Articles.Include(a => a.Category).Where(a => a.IsPublished == true).ToList();
            }
        }

        public int CountArticles()
        {
            using(var db=new NewsPortalContext())
            {
                return db.Articles.Where(a => a.IsPublished == true).Count();
            }
        }

        public List<Article> GetArticlesOfSameCategoryToNewsPortal(int? id)
        {
            using (var db = new NewsPortalContext())
            {
                return db.Articles.Include(a => a.Category)
                    .Where(a => (a.Category_Id == id && a.IsPublished) || a.Category.Parent_Id == id.ToString())
                    .ToList();
            }
        }

        public CategoryViewModel GetArticleOfNewsPortal(int id)
        {
            using (var db=new NewsPortalContext())
            {
                var model = new CategoryViewModel();
                var categories = db.Categories.Where(x => !x.IsDeleted).ToList();

                model.Categories = categories.Select(x => new CategoryItem()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Parent_Id = x.Parent_Id

                }).ToList();
                model.Articles = db.Articles.Where(a => a.Id == id && a.IsPublished).ToList();

                var comments = db.Comments.Where(c => c.Article_Id == id);

                model.Comments = comments.Select(x => new CommentViewModel()
                {
                    Id = x.Id,
                    Body = x.Body,
                    Article_Id = id,
                    Name = x.Name
                }).ToList();

                model.Article_Id = id;

                return model;
            }

        }
       
    }
}
