using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsPortal.BLL.Services;
using NewsPOrtal.DAL.ViewModels;
using System;
using System.Linq;

namespace NewsPortal.Controllers.Api
{
    [Route("api/newsportal")]
    [ApiController]
    public class NewsPortalApiController : ControllerBase
    {
        private readonly ArticleService _articleService = new ArticleService();
        private readonly CategoryService _categoryService = new CategoryService();
        private readonly CommentService _commentService = new CommentService();

        // GET /api/newsportal?page=0
        [HttpGet]
        public IActionResult Index([FromQuery] int page = 0)
        {
            const int PageSize = 4;
            var pubArticles = _articleService.GetArticlesOfNewsPortal();
            var count = _articleService.CountArticles();
            int maxPage = _articleService.GetMaxPageForArticles(count, PageSize);

            if (page > maxPage)
                page = 0;

            var data = _categoryService.GetCategoriesAndArticles(page, pubArticles);

            return Ok(new
            {
                currentPage = page,
                maxPage,
                data
            });
        }

        // GET /api/newsportal/category/{id}?page=0
        [HttpGet("category/{id}")]
        public IActionResult Category(int id, [FromQuery] int page = 0)
        {
            var categories = _categoryService.GetAllCategories();
            var exists = categories.FirstOrDefault(x => x.Id == id);
            if (exists == null)
                return NotFound(new { message = "Category not found." });

            const int PageSize = 6;
            var pubArticlesByCat = _articleService.GetArticlesOfSameCategoryToNewsPortal(id);
            var count = pubArticlesByCat.Count();
            int maxPage = _articleService.GetMaxPageForArticles(count, PageSize);

            if (page > maxPage)
                page = 0;

            var data = _categoryService.GetCategoriesAndArticlesOfSameCategory(id, page, pubArticlesByCat);

            return Ok(new
            {
                currentPage = page,
                maxPage,
                data
            });
        }

        // GET /api/newsportal/article/{id}
        [HttpGet("article/{id}")]
        public IActionResult Article(int id)
        {
            var article = _articleService.GetArticleOfNewsPortal(id);
            if (article == null)
                return NotFound(new { message = "Article not found." });

            return Ok(article);
        }

        // POST /api/newsportal/comment
        [HttpPost("comment")]
        public IActionResult CreateComment([FromBody] CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _commentService.AddComment(model);
            return Ok(new { message = "Comment added." });
        }

        // DELETE /api/newsportal/comment/{id}
        [HttpDelete("comment/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult DeleteComment(int id)
        {
            var comment = _commentService.FindCommentById(id);
            if (comment == null)
                return NotFound(new { message = "Comment not found." });

            _commentService.RemoveComment(comment);
            return Ok(new { message = "Comment deleted.", articleId = comment.Article_Id });
        }
    }
}
