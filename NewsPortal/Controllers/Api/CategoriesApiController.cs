using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsPortal.BLL.Services;
using NewsPortal.Common.Models.Requests.Category;

namespace NewsPortal.Controllers.Api
{
    [Route("api/categories")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class CategoriesApiController : ControllerBase
    {
        private readonly CategoryService _categoryService = new CategoryService();
        private readonly ArticleService _articleService = new ArticleService();

        // GET /api/categories
        [HttpGet]
        public IActionResult Index()
        {
            var categories = _categoryService.GetAllCategories();
            return Ok(categories);
        }

        // POST /api/categories
        [HttpPost]
        public IActionResult Create([FromBody] AddCategory category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _categoryService.AddCategoryCustomValidation(category, User);
            if (!result.IsSuccessfull)
                return BadRequest(new { errors = result });

            return Ok(new { message = "Category created." });
        }

        // DELETE /api/categories/{id}
        [HttpDelete("{id}")]
        public IActionResult SoftDelete(int id)
        {
            var category = _categoryService.FindCategoryById(id);
            if (category == null)
                return NotFound(new { message = "Category not found." });

            if (_categoryService.CheckIfCategoryIsEmpty(category))
            {
                _categoryService.RemoveCategory(category);
                return Ok(new { message = "Category deleted." });
            }

            return BadRequest(new { message = "Category is not empty and cannot be deleted." });
        }

        // GET /api/categories/{id}/articles — articles by category (Journalist sees own articles only)
        [HttpGet("{id}/articles")]
        [Authorize(Roles = "SuperAdmin,Journalist")]
        public IActionResult GetArticlesByCategory(int id)
        {
            var category = _categoryService.FindCategoryById(id);
            if (category == null)
                return NotFound(new { message = "Category not found." });

            if (User.IsInRole("Journalist"))
                return Ok(_articleService.GetArticlesOfUser(id, User));

            return Ok(_articleService.GetArticlesOfCategory(id));
        }
    }
}
