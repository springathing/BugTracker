using mblyakher_bugtracker.Models;
using mblyakher_bugtracker.Models.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mblyakher_bugtracker.Controllers
{
    [RequireHttps]
    public class AdminController : Universal
    {
        // GET: Admin
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            //if (Request.IsAuthenticated && User.IsInRole("Admin"))
                List<AdminUserViewModels> users = new List<AdminUserViewModels>();
                UserRoleHelper helper = new UserRoleHelper();
                foreach (var user in db.Users.ToList())
                {
                    var eachUser = new AdminUserViewModels();                
                    eachUser.User = user;
                    eachUser.SelectedRoles = helper.ListUserRoles(user.Id).ToArray();

                    users.Add(eachUser);
                }
                return View(users.OrderBy(u => u.User.LastName).ToList());
        }

        // GET: GetUserRoles
        //[RequireHttps]
        [Authorize(Roles = "Admin")]
        public ActionResult EditUserRoles(string Id)
        {
            var user = db.Users.Find(Id);
            var helper = new UserRoleHelper();
            var model = new AdminUserViewModels();
            model.User = user;
            model.SelectedRoles = helper.ListUserRoles(Id).ToArray();
            model.Roles = new MultiSelectList(db.Roles, "Name", "Name", model.SelectedRoles);
                                            // parameter is an IEnumerable, valuefield is what gets actually passed for each selected items
                                            // text field is what is shown, our roles pass a name and we show a name
            return View(model);

        }

        // POST: EditUserRoles
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditUserRoles(AdminUserViewModels model)
        {
            var user = db.Users.Find(model.User.Id);
            UserRoleHelper helper = new UserRoleHelper();

            foreach (var role in db.Roles.Select(r => r.Name).ToList())
            {
                helper.RemoveUserFromRole(user.Id, role);
            }

            if (model.SelectedRoles != null)
            {
                foreach (var role in model.SelectedRoles)
                {
                    helper.AddUserToRole(user.Id, role);
                }

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}