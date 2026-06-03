using NewsPortal.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NewsPOrtal.DAL.ViewModels;
using CategoryViewModel = NewsPOrtal.DAL.ViewModels.CategoryViewModel;
using NewsPOrtal.DAL.Models;
using System;

namespace NewsPortal.Controllers
{
    public class NewsPortalController : BaseController
    {
       
        private ArticleService articleService = new ArticleService();
        private CategoryService categoryService = new CategoryService();
        private CommentService commentService = new CommentService();

        // GET: NewsPortal
        public IActionResult Index(int page=0)
        {
            
            const int PageSize = 4;
            var pubArticles = articleService.GetArticlesOfNewsPortal();
            var count = articleService.CountArticles();

            int maxPage = articleService.GetMaxPageForArticles(count, PageSize);
            this.ViewBag.MaxPage = maxPage;
            this.ViewBag.CurrentPage = page;
            this.ViewBag.Page = page;

            if (page > maxPage)
            {
                return RedirectToAction("Index", new { page = 0 });
            }

            return View(categoryService.GetCategoriesAndArticles(page, pubArticles));
        }

        public IActionResult Category(int? id,int page=0)
        {
            if (id == null)
            {
                return StatusCode(400);
            }

            var categories = categoryService.GetAllCategories();

            var checkIsEgixt = categories.FirstOrDefault(x => x.Id == id);
            if (checkIsEgixt == null)
            {
                return NotFound();
            }

            const int PageSize = 6;
            var pubArticlesByCat = articleService.GetArticlesOfSameCategoryToNewsPortal(id);
            var count =pubArticlesByCat.Count();

            int maxPage = articleService.GetMaxPageForArticles(count, PageSize);
            this.ViewBag.MaxPage = maxPage;
            this.ViewBag.CurrentPage = page;
            this.ViewBag.Page = page;
            if (page > maxPage)
            {
                return RedirectToAction("Category", new { id = id, page = 0 });
            }

            
         

            return View("Category", categoryService.GetCategoriesAndArticlesOfSameCategory(id, page, pubArticlesByCat));

        }

        public IActionResult Article(int id)
        {

           
            return View("Article",articleService.GetArticleOfNewsPortal(id));
        }

        public IActionResult CreateComment(CategoryViewModel model)
        {
            var isAjax = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
            if (isAjax)
            {
                commentService.AddComment(model);
                return PartialView("_SingleComment",model);
            } else
            {
                var referer = Request.Headers.Referer.ToString();
                return !string.IsNullOrWhiteSpace(referer)
                    ? Redirect(referer)
                    : RedirectToAction(nameof(Index));
            }
            
        }
        
        [Authorize(Roles ="SuperAdmin")]
        public IActionResult DeleteComment(int id)
        {

            Comment comment = commentService.FindCommentById(id);
            commentService.RemoveComment(comment);

            return RedirectToAction("Article",new { id = comment.Article_Id });
        }

        public IActionResult Contact()
        {
            var model = new CategoryViewModel();
            var categories = categoryService.GetAllCategories();

            categoryService.AddCategoriesToViewModel(model);

            return View(model);
        }

        public IActionResult About()
        {
            var model = new CategoryViewModel();
            var categories = categoryService.GetAllCategories();

            categoryService.AddCategoriesToViewModel(model);

            return View(model);
        }
    }

}