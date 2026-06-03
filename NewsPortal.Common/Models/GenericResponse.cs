using NewsPortal.Common.Models.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NewsPortal.Common.Models
{
    public class GenericResponse
    {
        public GenericResponse()
        {
        }

        public GenericResponse(ValidationErrorCollection errors)
        {
            Errors = errors;
            IsSuccessfull = errors == null || !errors.Any() ;
        }

        public GenericResponse(Exception e)
        {
            IsSuccessfull = false;
            Errors = new ValidationErrorCollection() { e.Message };
        }


        public bool IsSuccessfull { get; set; }
        public ValidationErrorCollection Errors { get; set; }
    }

    public class GenericResponse<T> : GenericResponse
    {
        public T Result { get; set; }

        public GenericResponse()
        {

        }

        public GenericResponse(Exception e)
        {
            IsSuccessfull = false;
            Errors = new ValidationErrorCollection() { e.Message };
            Result = default(T);
        }

        public GenericResponse(string e)
        {
            IsSuccessfull = false;
            Errors = new ValidationErrorCollection() { e };
            Result = default(T);
        }
    }
}
