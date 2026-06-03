using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPOrtal.DAL.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your comment")]
        public string Body { get; set; }

        [ForeignKey("Article")]
        public int Article_Id { get; set; }
        public virtual Article Article { get; set; }
    }
}
