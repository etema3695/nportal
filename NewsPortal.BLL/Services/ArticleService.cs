using NewsPortal.Common.Models;
using NewsPortal.Common.Models.Requests.Article;
using NewsPortal.Common.Models.Validation;
using NewsPOrtal.DAL.Mapping;
using NewsPOrtal.DAL.Models;
using NewsPOrtal.DAL.Repositories;
using NewsPOrtal.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace NewsPortal.BLL.Services
{
    public class ArticleService : BaseService
    {
        private readonly ArticleRepository articleRepository=new ArticleRepository();

        public ArticleService()
        {

        }

       

        public void AddArticle(AddArticle request,IPrincipal User)
        {
            request.CreatedBy = User.Identity.Name;
            request.CreatedOn = DateTime.Now;
            string ImageTitle = Path.GetFileNameWithoutExtension(request.ImageTitle);
            string extension = Path.GetExtension(request.ImageTitle);
            ImageTitle = ImageTitle + extension;
            request.ImagePath = "~/Images/images/" + ImageTitle;
            articleRepository.AddArticle(request);

        }

        public Article FindArticleById(int? id)
        {
            
          return  articleRepository.FindArticleById(id);
        }

        public void RemoveArticle(Article request)
        {
            articleRepository.RemoveArticle(request);
        }

        public void PublishArticle(int id)
        {
            var article=articleRepository.FindArticleById(id);
            article.IsPublished = true;
            articleRepository.PublishArticle(article);

        }

       public GenericResponse<List<Article>> GetAllArticles(IPrincipal User)
        {
            return articleRepository.GetAllArticles(User);
        }


        public GenericResponse<Article> GetArticle(int? id)
        {
            return articleRepository.GetArticle(id);
        }


        public bool CheckIfArticleExistsForUser(Article article,IPrincipal User)
        {
            if (article == null || User.IsInRole("Journalist") && article.CreatedBy != User.Identity.Name)
            {
                return false;
            }
            else return true;

        }

        public bool CheckIfUserHasAcces(Article article,IPrincipal User)
        {
            if (User.IsInRole("Journalist") && article.CreatedBy == User.Identity.Name)
            {
                return false;
            }
            else return true;
        }

        public void EditArticle(Article request, Stream fileStream, string fileName, string imagesPhysicalPath)
        {
            if (fileStream != null && !string.IsNullOrEmpty(fileName))
            {
                string safeFileName = Path.GetFileName(fileName);
                string fullPath = Path.Combine(imagesPhysicalPath, safeFileName);

                using (var fs = new FileStream(fullPath, FileMode.Create))
                {
                    fileStream.CopyTo(fs);
                }

                request.ImageTitle = safeFileName;
                request.ImagePath = "~/Images/images/" + safeFileName;

                articleRepository.EditArticle(request);
            }
            else
            {
                articleRepository.EditArticle(request);
            }
        }

        public List<Article>GetArticlesOfCategory(int? id)
        {
            return articleRepository.GetArticlesOfSameCategory(id);
        }

        public List<Article>GetArticlesOfUser(int? id,IPrincipal User)
        {
            return articleRepository.GetArticlesOfUser(id, User);
        }

        public List<Article> GetArticlesOfNewsPortal()
        {
            return articleRepository.GetArticlesInNewsPortal();
        }

        public int CountArticles()
        {
            return articleRepository.CountArticles();
        }

        public List<Article> GetArticlesOfSameCategoryToNewsPortal(int? id)
        {
            return articleRepository.GetArticlesOfSameCategoryToNewsPortal(id);
        }

        public int GetMaxPageForArticles(int count, int PageSize)
        {
            if (count == 0) return 0;
            return (count / PageSize) - (count % PageSize == 0 ? 1 : 0);
        }

        public CategoryViewModel GetArticleOfNewsPortal(int id)
        {
            return articleRepository.GetArticleOfNewsPortal(id);
        }

        protected ValidationErrorCollection ValidateProduct(AddArticle articleToValidate)
        {
            var errors = new ValidationErrorCollection();
            if (string.IsNullOrEmpty(articleToValidate.Title))
            {
                errors.Add("Title", "Title field is required !.");
            }
            else
            {
                if (articleToValidate.Title.Length > 255)
                {
                    errors.Add("Title", "Max title is 255 chars!.");
                }
            }
            if (articleToValidate.Description == null)
                errors.Add("Description", "Please enter short description.");
            if (articleToValidate.Body == null)
                errors.Add("Body", "Please enter body.");
            if (articleToValidate.ImageTitle == null)
                errors.Add("ImageTitle", "Please upload image.");
            return errors;
            
        }

        public GenericResponse AddArticleCustomValidation(AddArticle request, IPrincipal User)
        {
            var error = ValidateProduct(request);
            if (error != null && error.Count() > 0)
            {
                return new GenericResponse(error);
            }
            request.CreatedBy = User.Identity.Name;
            request.CreatedOn = DateTime.Now;
            string ImageTitle = Path.GetFileNameWithoutExtension(request.ImageTitle);
            string extension = Path.GetExtension(request.ImageTitle);
            ImageTitle = ImageTitle + extension;
            request.ImagePath = "~/Images/images/" + ImageTitle;
            articleRepository.AddArticle(request);

            return new GenericResponse()
            {
                IsSuccessfull = true
            };
        }

    }
}
