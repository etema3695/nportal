using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewsPortal.Models;
using System.IO;
using NewsPortal.BLL.Services;
using NewsPortal.Common.Models.Requests.Article;
using NewsPOrtal.DAL.Models;
using NewsPOrtal.DAL.Mapping;
using System.ComponentModel.DataAnnotations;
using System;
using NewsPortal.Common.Models;
using System.Collections.Generic;

namespace NewsPortal.Controllers
{
    [Authorize(Roles = "SuperAdmin,Journalist")]
    public class ArticlesController : BaseController
    {
        private readonly ArticleService articleService = new ArticleService();
        private readonly CategoryService categoryService = new CategoryService();
        private readonly IWebHostEnvironment _env;

        public ArticlesController(IWebHostEnvironment env)
        {
            _env = env;
        }
        

        // GET: Articles
        public IActionResult Index()
        {
            var article = articleService.GetAllArticles(User);
            if (!article.IsSuccessfull)
            {

                throw new ArgumentNullException();
            }
            return View(article.Result);
        }

        // GET: Articles/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return StatusCode(400);
            }

            var article = articleService.GetArticle(id);

            if (!article.IsSuccessfull)
            {
                throw new ArgumentNullException();
            }


            if (articleService.CheckIfArticleExistsForUser(article.Result, User) == false)
            {
                return NotFound();
            }

            return View(article.Result);
        }

        // GET: Articles/Create
        public IActionResult Create()
        { 
            ViewBag.Category_Id = new SelectList(categoryService.GetAllCategories(), "Id", "Code");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AddArticle article)
        {

            var addArticle = articleService.AddArticleCustomValidation(article, User);
            if (!addArticle.IsSuccessfull)
            {
                AddCustomValidationToModelState(addArticle);
            }

            //if (ModelState.IsValid)
            //{
            //    articleService.AddArticle(article, User);
            //    return RedirectToAction("Index");
            //}

            ViewBag.Category_Id = new SelectList(categoryService.GetAllCategories(), "Id", "Code", article.Category_Id);
            return View();
        }

        // GET: Articles/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return StatusCode(400);
            }
            var article = articleService.FindArticleById(id);

            if (articleService.CheckIfArticleExistsForUser(article, User) == false)
            {
                return NotFound();
            }

            if (article.IsPublished == true)
            {
                if (articleService.CheckIfUserHasAcces(article,User)==false)
                {
                    return StatusCode(403);
                }
            }
            ViewBag.Category_Id = new SelectList(categoryService.GetAllCategories(), "Id", "Code", article.Category_Id);
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(NewsPOrtal.DAL.Models.Article article, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string imagesPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "Images", "images");
                if (!Directory.Exists(imagesPath)) Directory.CreateDirectory(imagesPath);
                articleService.EditArticle(article, file?.OpenReadStream(), file?.FileName, imagesPath);
                return RedirectToAction("Index");
            }
            ViewBag.Category_Id = new SelectList(categoryService.GetAllCategories(), "Id", "Code", article.Category_Id);
            return View(article);
        }

        // GET: Articles/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return StatusCode(400);
            }
            var article = articleService.GetArticle(id);
           

            if (articleService.CheckIfArticleExistsForUser(article.Result, User) == false)
            {
                return NotFound();
            }

            if (article.Result.IsPublished == true)
            {
                if (articleService.CheckIfUserHasAcces(article.Result, User) == false)
                {
                    return StatusCode(403);
                }
            }
            return View(article.Result);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            

            var article=articleService.FindArticleById(id);
            
            articleService.RemoveArticle(article);
           
            return RedirectToAction("Index");
        }

       
        [Authorize(Roles ="SuperAdmin")]
        public IActionResult Publish(int id)
        {

            articleService.PublishArticle(id);

            return RedirectToAction("Index");
        }



    }
}
