using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPortal.Common.Models.Validation
{
    public class ValidationErrorCollection:ICollection<ValidationError>
    {
        public List<ValidationError> Errors { get; set; }
        public ValidationErrorCollection()
        {
            Errors = new List<ValidationError>();
        }

        public int Count => Errors.Count;

        public bool IsReadOnly => false;

        public void Add(string message)
        {
            Errors.Add(new ValidationError(message));
        }
        public void Add(string field,string message)
        {
            Errors.Add(new ValidationError(field, message));
        }

        public void Add(ValidationError item)
        {
            Errors.Add(item);
        }

        public void Clear()
        {
            Errors.Clear();
        }

        public bool Contains(ValidationError item)
        {
            return Errors.Contains(item);
        }

        public void CopyTo(ValidationError[] array, int arrayIndex)
        {
            Errors.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ValidationError> GetEnumerator()
        {
            return Errors.GetEnumerator();
        }

        public bool Remove(ValidationError item)
        {
            return Errors.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
