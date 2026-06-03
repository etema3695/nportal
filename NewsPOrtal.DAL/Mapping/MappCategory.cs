using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsPortal.Common.Models.Requests.Category;
using NewsPOrtal.DAL.Models;

namespace NewsPOrtal.DAL.Mapping
{
    public static class MappCategory
    {
        public static Category Map(AddCategory addCategory)
        {
            if (addCategory == null)
            {
                return null;
            }
            else
            {
                return new Category()
                {
                    Code = addCategory.Name,
                    Parent_Id = addCategory.ParentId,
                    CreatedOn = addCategory.CreatedOn,
                    CreatedBy=addCategory.CreatedBy
                   
                };
            }
        }
    }
}
