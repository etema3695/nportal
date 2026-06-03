using NewsPortal.Common.Models;
using NewsPortal.Models;
using NewsPortal.ViewModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace NewsPortal.Controllers
{
    public class BaseController : Controller
    {
        public void AddCategoriesToViewModel(BaseVieModel baseViewModel)
        {
            using (var db = new ApplicationDbContext())
            {
                baseViewModel.Categories = db.Categories.Where(x => !x.IsDeleted).Select(x => new CategoryItem()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Parent_Id = x.Parent_Id

                }).ToList();
            }
        }

        protected void AddCustomValidationToModelState(GenericResponse response)
        {
            if (response.Errors != null && response.Errors.Count() > 0)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(error.Field ?? string.Empty, error.Message);
                }
            }
        }


    }
}