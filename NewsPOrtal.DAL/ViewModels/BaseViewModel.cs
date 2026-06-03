using NewsPOrtal.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPOrtal.DAL.ViewModels
{
    public class BaseViewModel:CommentViewModel
    {
        public List<CategoryItem> Categories { get; set; }
        
    }
}
