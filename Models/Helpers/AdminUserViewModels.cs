using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mblyakher_bugtracker.Models.Helpers
{
    // using this because we can only send one model to a page with @model at the top but in this case we want to send multiple user 
    // objects and the roles to the page at once so we make it our admin index using this helper class
    public class AdminUserViewModels
    {
        // passing in the user and the roles
        public ApplicationUser User { get; set; }
        public MultiSelectList Roles { get; set; }  // using an array instead of a list because we're sending multiple strings, we'd
        public string[] SelectedRoles { get; set; } // use list for a more complicated list like a list of objects
    }
}