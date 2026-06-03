using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPortal.Common.Models.Validation
{
    public class ValidationError
    {
        public string Field { get; set; }
        public string Message { get; set; }
        public ValidationError(string message)
        {
            Message = message;
        }
        public ValidationError(string field,string message)
        {
            Message = message;
            Field = field;
        }
    }
    
}
