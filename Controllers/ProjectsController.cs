using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mblyakher_bugtracker.Models;
using mblyakher_bugtracker.Models.CodeFirst;
using mblyakher_bugtracker.Models.Helpers;
using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;

namespace mblyakher_bugtracker.Controllers
{
    [RequireHttps]
    public class ProjectsController : Universal
    {
        // GET: Projects
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Index()
        {
            return View(db.Projects.ToList());
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        public const string roleProjectManager = "Project Manager";
        

        // GET: Projects/Create
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Create()
        {
            //var projectMangerId = context.Roles.SingleOrDefault(u => u.Name == "Project Manager").ID;
            //var lstUsersProjectManagers = db.AspNetUserRoles.Where(x => x.RoleId == projectMangerId).Select(x => x.UserId).ToList();
            //var usersToDisplay = db.Users.Where(lstUsersProjectManagers.Contains(x.Id)).ToList();
            //var mySelectList = new SelectList(usersToDisplay, "ID", "Name");

            //ApplicationDbContext context = new ApplicationDbContext();
            //Project project = db.Projects.Find(ProjectId);
            //var projectUsers = context.Users.Where(u => u.Projects.Any(p => p.Title == project.Title));
            //var role = context.Roles.SingleOrDefault(u => u.Name == "Project Manager");
            //var usersInRole = context.Users.Where(u => u.Roles.Any(r => (r.RoleId == role.Id)));
            //var displayUsers = usersInRole.Where(u => u.Projects.Any(p => (p.Title == project.Title)));
            //ViewBag.ListOfProjectManagers = new SelectList(displayUsers, "Id", "DisplayName");

            //For the ProjectsController in Create Action
            var role = db.Roles.SingleOrDefault(u => u.Name == "Project Manager");
            var usersInRole = db.Users.Where(u => u.Roles.Any(r => (r.RoleId == role.Id)));
            ViewBag.AssignedToId = new SelectList(usersInRole, "Id", "FullName");

            //ViewBag.AssignedToId = new SelectList(db.Users, "Id", "FullName");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Create([Bind(Include = "Id,Created,Updated,Title,Description,AuthorId,AssignedToId")] Project project)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                project.AuthorId = user.FullName;
                project.Created = DateTimeOffset.Now;
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var role = db.Roles.SingleOrDefault(u => u.Name == "Project Manager");
            var usersInRole = db.Users.Where(u => u.Roles.Any(r => (r.RoleId == role.Id)));
            ViewBag.AssignedToId = new SelectList(usersInRole, "Id", "FullName", project.AssignedToId);
            //ViewBag.AssignedToId = new SelectList(db.Users, "Id", "FullName", project.AssignedToId);
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Edit(int? id)
        {
            var role = db.Roles.SingleOrDefault(u => u.Name == "Project Manager");
            var usersInRole = db.Users.Where(u => u.Roles.Any(r => (r.RoleId == role.Id)));
            ViewBag.AssignedToId = new SelectList(usersInRole, "Id", "FullName");
            //ViewBag.AssignedToId = new SelectList(db.Users, "Id", "FullName");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Description,AssignedToId")] Project project)
        {
            var role = db.Roles.SingleOrDefault(u => u.Name == "Project Manager");
            var usersInRole = db.Users.Where(u => u.Roles.Any(r => (r.RoleId == role.Id)));
            ViewBag.AssignedToId = new SelectList(usersInRole, "Id", "FullName");
            //ViewBag.AssignedToId = new SelectList(db.Users, "Id", "FullName");
            if (ModelState.IsValid)
            {
                project.Updated = DateTimeOffset.Now;
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: EditProjectUsers
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult ProjectUser(int? id)
        {
            var project = db.Projects.Find(id);
            ProjectUserViewModels projectuserVM = new ProjectUserViewModels();
            projectuserVM.AssignProject = project;
            projectuserVM.AssignProjectId = Convert.ToInt32(project.Id); // primary keys to be int not hash-code bc db to auto increment
            projectuserVM.SelectedUsers = project.Users.Select(u => u.Id).ToArray();
            projectuserVM.Users = new MultiSelectList(db.Users.ToList(), "Id", "FullName", projectuserVM.SelectedUsers);
            // MultiSelectList parameters: collection of objects used for select list (user for us), inside user table we have diff properties that we submit by like Id or FirstName, display is FirstNames, highlighted selected users
            return View(projectuserVM); // how does this view return work? View(model) for example: passes in an object
        }
        // project and user is a many to many relationship, the project user view model is the projectuser table that holds the cross reference between projects and its users
        // we would need a dbset<ProjectUser> in the identitymodel potentially

        // POST: EditProjectUsers
        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult ProjectUser(ProjectUserViewModels model)
        {
            // submit assign and it goes through here, we need to go through users assigned in database and put in the users selected
            ProjectAssignHelper projectassignhelper = new ProjectAssignHelper();
            //var project = db.Projects.Find(model.AssignProject.Id); No need for this since we don't need to find project in db by Id
            foreach (var userId in db.Users.Select(r => r.Id).ToList()) // remove all users from project
            {
                projectassignhelper.RemoveUserFromProject(userId, model.AssignProjectId);
            }
            foreach (var userId in model.SelectedUsers) // add back the ones you want
            {
                projectassignhelper.AddUserToProject(userId, model.AssignProjectId);
            }

            return RedirectToAction("Index");

        }

        // GET: Assigned Projects to user
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult AssignedProjects(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var user = db.Users.Find(User.Identity.GetUserId());

            return View(db.Projects.Where(a => user.Id.Contains(a.AssignedToId)).OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
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
