using NewsPortal.Common.Models.Requests.Category;
using NewsPOrtal.DAL.Mapping;
using NewsPOrtal.DAL.Models;
using NewsPOrtal.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPortal.DAL.Repositories
{
    public class CategoryRepository 
    {
        public void AddCategory(AddCategory addCategory)
        {
            using (var db = new NewsPortalContext())
            {
                var entity = MappCategory.Map(addCategory);
                db.Categories.Add(entity);
                db.SaveChanges();
            }

        }

        public Category FindCategoryById(int? id)
        {
            using (var db = new NewsPortalContext())
            {

                AddCategory addCategory = new AddCategory();
                var category = MappCategory.Map(addCategory);

                category = db.Categories.Find(id);

                return category;
            }

        }

        public void RemoveCategory(Category category)
        {
            using (var db = new NewsPortalContext())
            {
                db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                category.IsDeleted = true;
                db.SaveChanges();
            }

        }

        public bool CheckIfCategoryIsEmpty(Category category)
        {
            using (var db = new NewsPortalContext())
            {

                var article = db.Articles.Where(a => a.Category_Id == category.Id);
                if (!article.Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public List<Category> GetAllCategories()
        {
            using (var db = new NewsPortalContext())
            {
                return db.Categories.Where(c => c.IsDeleted == false).ToList();
            }

        }
     
        public void AddCategoriesToViewModel(CategoryViewModel categoryViewModel)
        {
            using (var db = new NewsPortalContext())
            {
                categoryViewModel.Categories = db.Categories.Where(x => !x.IsDeleted).Select(x => new CategoryItem()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Parent_Id = x.Parent_Id

                }).ToList();
            }
        }

        public CategoryViewModel GetCategoriesAndArticles(int page,List<Article> pubArticles)
        {
            
            

            using (var db = new NewsPortalContext())
            {
                
                const int PageSize = 4;

                var model = new CategoryViewModel()
                {
                   

                    Categories = db.Categories.Where(x => !x.IsDeleted).Select(x => new CategoryItem()
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Parent_Id = x.Parent_Id

                    }).ToList(),
                    Articles = pubArticles.OrderBy(o => o.Id).Skip(page * PageSize).Take(PageSize).ToList(),
                    
                };
                
                return model;
            }

           

        }

        public CategoryViewModel GetCategoriesAndArticlesOfSameCategory(int? id,int page,List<Article> pubArticlesByCat)
        {
            using (var db = new NewsPortalContext())
            {

                const int PageSize = 6;

                var model = new CategoryViewModel()
                {


                    Categories = db.Categories.Where(x => !x.IsDeleted).Select(x => new CategoryItem()
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Parent_Id = x.Parent_Id

                    }).ToList(),
                    Articles = pubArticlesByCat.OrderBy(o => o.Id).Skip(page * PageSize).Take(PageSize).ToList(),

                };

                return model;
            }

        }
    }
}
