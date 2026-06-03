using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NewsPortal.Models;

namespace NewsPortal.ViewModel
{
    public class UserInRoleViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}