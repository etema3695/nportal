using NewsPortal.Common.Models;
using NewsPortal.Common.Models.Requests.Category;
using NewsPortal.Common.Models.Validation;
using NewsPortal.DAL.Repositories;
using NewsPOrtal.DAL.Models;
using NewsPOrtal.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace NewsPortal.BLL.Services
{
    public class CategoryService : BaseService
    {
        private readonly CategoryRepository categoryRepository = new CategoryRepository();


        public void AddCategory(AddCategory request, IPrincipal User)
        {
            request.CreatedBy = User.Identity.Name;
            request.CreatedOn = DateTime.Now;
            categoryRepository.AddCategory(request);
        }

        public Category FindCategoryById(int? id)
        {

            return categoryRepository.FindCategoryById(id);
        }

        public void RemoveCategory(Category request)
        {

            categoryRepository.RemoveCategory(request);

        }

        public bool CheckIfCategoryIsEmpty(Category request)
        {
            return categoryRepository.CheckIfCategoryIsEmpty(request);
        }

        public List<Category> GetAllCategories()
        {
            return categoryRepository.GetAllCategories();
        }

        public void AddCategoriesToViewModel(CategoryViewModel categoryViewModel)
        {
            categoryRepository.AddCategoriesToViewModel(categoryViewModel);
        }

        public CategoryViewModel GetCategoriesAndArticles(int page, List<Article>pubArticles)
        {
            return categoryRepository.GetCategoriesAndArticles(page,pubArticles);
        }

        public CategoryViewModel GetCategoriesAndArticlesOfSameCategory(int? id,int page, List<Article> pubArticlesByCat)
        {
            return categoryRepository.GetCategoriesAndArticlesOfSameCategory(id, page, pubArticlesByCat);
        }

        protected ValidationErrorCollection ValidateProduct(AddCategory categoryToValidate)
        {
            var errors = new ValidationErrorCollection();
            if (string.IsNullOrEmpty(categoryToValidate.Name))
            {
                errors.Add("Name", "Please enter Category name !.");
            }
            else
            {
                if (categoryToValidate.Name.Length > 255)
                {
                    errors.Add("Title", "Max title is 255 chars!.");
                }
            }
            return errors;

        }

        public GenericResponse AddCategoryCustomValidation(AddCategory request, IPrincipal User)
        {
            var error = ValidateProduct(request);
            if (error != null && error.Count() > 0)
            {
                return new GenericResponse(error);
            }

            request.CreatedBy = User.Identity.Name;
            request.CreatedOn = DateTime.Now;
            categoryRepository.AddCategory(request);

            return new GenericResponse()
            {
                IsSuccessfull = true
            };

        }
    }
}

