using NewsPortal.Common.Models.Requests.Category;
using NewsPortal.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsPortal.Mapping
{
    public static class MapCategory
    {
        public static AddCategory Map(CategoryViewModel categoryViewModel)
        {
            if (categoryViewModel == null)
                return null;
            return new AddCategory()
            {

                Name = categoryViewModel.Name,
                ParentId = categoryViewModel.ParentId
            };
        }
    }
}