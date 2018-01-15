using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mblyakher_bugtracker.Models.Helpers
{
    public class Universal: Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (User.Identity.IsAuthenticated)
            {
                var user = db.Users.Find(User.Identity.GetUserId()); // variables accessible for each view

                ViewBag.FirstName = user.FirstName;
                ViewBag.LastName = user.LastName;
                ViewBag.FullName = user.FullName;

                ViewBag.Projects = db.Projects.AsNoTracking().Count().ToString() + " Project(s)";
                ViewBag.Tickets = db.Tickets.AsNoTracking().Count().ToString() + " Ticket(s)";
                ViewBag.RegisteredUsers = db.Users.Count().ToString() + " Registered User(s)";
            }
        }
    }
}

//        ViewBag.CartItems = db.CartItems.AsNoTracking().Where(c => c.CustomerId == user.Id).ToList().Count().ToString() + " Item(s)";
//        // AsNoTracking() leaves no trail so accessing the database isn't left open
//        // OnActionExecuting, everything happening in controller is done at same time, so on action executing gets executed after your controller 
//        decimal Total = 0;
//        foreach (var item in db.CartItems.AsNoTracking().Where(c => c.CustomerId == user.Id).ToList())
//        {
//            Total += item.Count * item.Item.Price;
//        }
//        ViewBag.CartTotal = Total;
//    }
//    else
//    {
//        ViewBag.CartItems = "Please Log In";
//    }
//}

    //<div id = "cartsearch" class="row">                    
    //                <div style = "float:right; font-size:25px; padding-left:8px;" >
    //                    < a href="@Url.Action("Index", "CartItems")"><i id = "iconshop" data-toggle="tooltip" data-placement="bottom" title="@ViewBag.CartItems" class="fa fa-shopping-cart w3-hover-opacity w3-large"></i></a>
    //                    <i id = "iconsearch" class="fa fa-search w3-hover-opacity w3-large"></i>
    //                    <input id = "searchbar" type="text" placeholder="Search..." />
    //                </div>
          
    //            </div>