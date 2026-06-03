using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPortal.Common.Models.Requests.Comment
{
    public class AddComment
    {
        public string Name { get; set; }
        public string Body { get; set; }
        public int Article_Id { get; set; }
    }
}
