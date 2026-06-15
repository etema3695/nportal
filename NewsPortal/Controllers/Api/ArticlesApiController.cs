using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsPortal.BLL.Services;
using NewsPortal.Common.Models.Requests.Article;
using NewsPOrtal.DAL.Models;
using System;
using System.IO;

namespace NewsPortal.Controllers.Api
{
    [Route("api/articles")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin,Journalist")]
    public class ArticlesApiController : ControllerBase
    {
        private readonly ArticleService _articleService = new ArticleService();
        private readonly CategoryService _categoryService = new CategoryService();
        private readonly IWebHostEnvironment _env;

        public ArticlesApiController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // GET /api/articles
        [HttpGet]
        public IActionResult Index()
        {
            var result = _articleService.GetAllArticles(User);
            if (!result.IsSuccessfull)
                return StatusCode(500, new { message = "Could not retrieve articles." });

            return Ok(result.Result);
        }

        // GET /api/articles/{id}
        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            var result = _articleService.GetArticle(id);
            if (!result.IsSuccessfull)
                return NotFound(new { message = "Article not found." });

            if (!_articleService.CheckIfArticleExistsForUser(result.Result, User))
                return NotFound(new { message = "Article not found." });

            return Ok(result.Result);
        }

        // GET /api/articles/categories — helper for dropdowns
        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            var categories = _categoryService.GetAllCategories();
            return Ok(categories);
        }

        // POST /api/articles
        [HttpPost]
        public IActionResult Create([FromForm] AddArticle article, IFormFile file)
        {
            var result = _articleService.AddArticleCustomValidation(article, User);
            if (!result.IsSuccessfull)
                return BadRequest(new { errors = result });

            return Ok(new { message = "Article created." });
        }

        // PUT /api/articles/{id}
        [HttpPut("{id}")]
        public IActionResult Edit(int id, [FromForm] Article article, IFormFile file)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string imagesPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "Images", "images");
            if (!Directory.Exists(imagesPath))
                Directory.CreateDirectory(imagesPath);

            _articleService.EditArticle(article, file?.OpenReadStream(), file?.FileName, imagesPath);
            return Ok(new { message = "Article updated." });
        }

        // DELETE /api/articles/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _articleService.GetArticle(id);
            if (!result.IsSuccessfull)
                return NotFound(new { message = "Article not found." });

            if (!_articleService.CheckIfArticleExistsForUser(result.Result, User))
                return NotFound(new { message = "Article not found." });

            if (result.Result.IsPublished &&
                !_articleService.CheckIfUserHasAcces(result.Result, User))
                return StatusCode(403, new { message = "Access denied." });

            var article = _articleService.FindArticleById(id);
            _articleService.RemoveArticle(article);
            return Ok(new { message = "Article deleted." });
        }

        // POST /api/articles/{id}/publish
        [HttpPost("{id}/publish")]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Publish(int id)
        {
            _articleService.PublishArticle(id);
            return Ok(new { message = "Article published." });
        }
    }
}
