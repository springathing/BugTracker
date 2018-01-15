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
using System.IO;
using System.Data.Entity.Infrastructure;
using System.Reflection;

namespace mblyakher_bugtracker.Controllers
{
    [RequireHttps]
    public class TicketsController : Universal
    {
        // GET: Tickets
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Index(int? page)
        {
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            var tickets = db.Tickets.Include(t => t.AssignToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(tickets.OrderBy(t => t.Id).ToPagedList(pageNumber, pageSize));
        }

        // POST
        [HttpPost]
        public ActionResult Index(string searchStr, int? page)
        {
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(db.Tickets.Where(i => i.Description.Contains(searchStr) || i.Title.Contains(searchStr) || i.Project.Title.Contains(searchStr) ||
            i.AssignToUser.FirstName.Contains(searchStr) || i.AssignToUser.LastName.Contains(searchStr) || i.TicketPriority.Name.Contains(searchStr) ||
            i.TicketType.Name.Contains(searchStr)).OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
        }

        // GET: Tickets/Details/5
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Admin, Project Manager, Submitter")]
        public ActionResult Create(int? id)
        {
            //var projects = db.Projects.Where(p => p.AssignedToId == User.Identity.GetUserId());

            //ViewBag.AssignToUserId = new SelectList(db.Users, "Id", "FirstName");
            //ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName");
            //if (projects != null)
            //{
            //    ViewBag.ProjectId = new SelectList(projects, "Id", "Title");
            //}
            //ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            //ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            //ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            //return View();

            // Since I'm filtering the projects in assigned projects I don't need to include project dropdown for choosing a project to create a ticket for
            var role = db.Roles.SingleOrDefault(u => u.Name == "Developer");
            var usersInRole = db.Users.Where(u => u.Roles.Any(r => (r.RoleId == role.Id)));
            ViewBag.AssignToUserId = new SelectList(usersInRole, "Id", "FullName");

            //ViewBag.AssignToUserId = new SelectList(db.Users, "Id", "FullName");
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FullName");
            ViewBag.ProjectId = id;
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Create([Bind(Include = "Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var status = db.TicketStatuses.FirstOrDefault(s => s.Name.Equals("Unassigned")) as TicketStatus;
                // first or default record that matches // casting the return value that goes into status as a TicketStatus type which is the entity, otherwise it returns the whole DbSet
                if (status != null)
                {
                    ticket.TicketStatusId = status.Id;
                }
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.Created = DateTimeOffset.Now;
                db.Tickets.Add(ticket);
                db.SaveChanges();

                ApplicationUser currentUser = db.Users.FirstOrDefault(u => u.UserName.Equals(User.Identity.Name));
                TicketHistory newHistory = new TicketHistory();
                newHistory.Property = "TICKET CREATED";
                newHistory.Created = DateTimeOffset.Now;
                newHistory.AuthorId = currentUser.Id;
                newHistory.TicketId = ticket.Id;
                db.TicketHistories.Add(newHistory);
                db.SaveChanges();

                return RedirectToAction("AssignedTickets");
            }

            var role = db.Roles.SingleOrDefault(u => u.Name == "Developer");
            var usersInRole = db.Users.Where(u => u.Roles.Any(r => (r.RoleId == role.Id)));
            ViewBag.AssignToUserId = new SelectList(usersInRole, "Id", "FullName", ticket.AssignToUserId);
            //ViewBag.AssignToUserId = new SelectList(db.Users, "Id", "FullName", ticket.AssignToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FullName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }


        // GET: Tickets/Edit/5
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            var role = db.Roles.SingleOrDefault(u => u.Name == "Developer");
            var usersInRole = db.Users.Where(u => u.Roles.Any(r => (r.RoleId == role.Id)));
            ViewBag.AssignToUserId = new SelectList(usersInRole, "Id", "FullName", ticket.AssignToUserId);
            //ViewBag.AssignToUserId = new SelectList(db.Users, "Id", "FullName", ticket.AssignToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FullName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignToUserId")] Ticket newTicket)
        {
            var userId = User.Identity.GetUserId();
            var userName = db.Users.Find(User.Identity.GetUserId()).FullName;
            var user = db.Users.Find(User.Identity.GetUserId());

            //get a non-proxy oldTicket
            var dbNoTrack = new ApplicationDbContext();
            ((IObjectContextAdapter)dbNoTrack).ObjectContext.ContextOptions.ProxyCreationEnabled = false;
            var oldTicket = dbNoTrack.Tickets.Find(newTicket.Id);
            //Ticket oldTicket = db.Tickets.Find(newTicket.Id); // this is just using the same ticket
            ApplicationUser currentUser = db.Users.FirstOrDefault(u => u.UserName.Equals(User.Identity.Name));
            if (ModelState.IsValid)
            {
                // Check & Record Changes
                newTicket.Updated = DateTimeOffset.Now;
                db.Entry(newTicket).State = EntityState.Modified;
                db.SaveChanges();
                // get rid of non-proxy old ticket
                CheckDifferingProperties(oldTicket, newTicket, currentUser);
                dbNoTrack.Dispose();

                return RedirectToAction("AssignedTickets");
            }

            var role = db.Roles.SingleOrDefault(u => u.Name == "Developer");
            var usersInRole = db.Users.Where(u => u.Roles.Any(r => (r.RoleId == role.Id)));
            ViewBag.AssignToUserId = new SelectList(usersInRole, "Id", "FullName", newTicket.AssignToUserId);

            //ViewBag.AssignToUserId = new SelectList(db.Users, "Id", "FullName", newTicket.AssignToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FullName", newTicket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", newTicket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", newTicket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", newTicket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", newTicket.TicketTypeId);
            return View(newTicket);
        }

        private void CheckDifferingProperties(Ticket oldTicket, Ticket newTicket, ApplicationUser currentUser)
        {
            var userName = db.Users.Find(User.Identity.GetUserId()).FullName;
            if (oldTicket.Title != newTicket.Title)
            {
                TicketHistory newHistory = new TicketHistory();
                newHistory.AuthorId = currentUser.Id;
                newHistory.TicketId = oldTicket.Id;
                newHistory.Property = "TICKET TITLE UPDATED";
                newHistory.Created = DateTimeOffset.Now;
                newHistory.OldValue = oldTicket.Title;
                newHistory.NewValue = newTicket.Title;
                db.TicketHistories.Add(newHistory);
                db.SaveChanges();

                Notification newNotification = new Notification();
                newNotification.TicketId = newTicket.Id;
                newNotification.Message = "A change to the Title was made by " + userName;
                newNotification.Created = DateTimeOffset.Now;
                newNotification.UserId = newTicket.AssignToUserId;
                db.Notifications.Add(newNotification);
                db.SaveChanges();
            }

            if (oldTicket.Description != newTicket.Description)
            {
                TicketHistory newHistory = new TicketHistory();
                newHistory.AuthorId = currentUser.Id;
                newHistory.TicketId = oldTicket.Id;
                newHistory.Property = "TICKET DESCRIPTION UPDATED";
                newHistory.Created = DateTimeOffset.Now;
                newHistory.OldValue = oldTicket.Description;
                newHistory.NewValue = newTicket.Description;
                db.TicketHistories.Add(newHistory);
                db.SaveChanges();

                Notification newNotification = new Notification();
                newNotification.TicketId = newTicket.Id;
                newNotification.Message = "A change to the Description was made by " + userName;
                newNotification.Created = DateTimeOffset.Now;
                newNotification.UserId = newTicket.AssignToUserId;
                db.Notifications.Add(newNotification);
                db.SaveChanges();
            }
            var newAssigned = db.Users.Find(newTicket.AssignToUserId);
            var oldAssigned = db.Users.Find(oldTicket.AssignToUserId);
            if (oldTicket.AssignToUserId != null)
            {
                if (oldTicket.AssignToUserId != newTicket.AssignToUserId)
                {
                    TicketHistory newHistory = new TicketHistory();
                    newHistory.AuthorId = currentUser.Id;
                    newHistory.TicketId = oldTicket.Id;
                    newHistory.Property = "TICKET ASSIGNMENT UPDATED";
                    newHistory.Created = DateTimeOffset.Now;
                    if (newAssigned != null && oldAssigned != null)
                    {
                        newHistory.OldValue = oldAssigned.FullName;
                        newHistory.NewValue = newAssigned.FullName;
                    }
                    db.TicketHistories.Add(newHistory);
                    db.SaveChanges();


                    Notification newNotification = new Notification();
                    newNotification.TicketId = newTicket.Id;
                    newNotification.Message = "A change to the Assigned User was made by " + userName;
                    newNotification.Created = DateTimeOffset.Now;
                    newNotification.UserId = newTicket.AssignToUserId;
                    db.Notifications.Add(newNotification);
                    db.SaveChanges();
                }
            }

            var newType = db.TicketTypes.Find(newTicket.TicketTypeId);
            var oldType = db.TicketTypes.Find(oldTicket.TicketTypeId);
            if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
            {
                TicketHistory newHistory = new TicketHistory();
                newHistory.AuthorId = currentUser.Id;
                newHistory.TicketId = oldTicket.Id;
                newHistory.Property = "TICKET TYPE UPDATED";
                newHistory.Created = DateTimeOffset.Now;
                if (newType != null && oldType != null)
                {
                    newHistory.OldValue = oldType.Name;
                    newHistory.NewValue = newType.Name;
                }          
                db.TicketHistories.Add(newHistory);
                db.SaveChanges();

                Notification newNotification = new Notification();
                newNotification.TicketId = newTicket.Id;
                newNotification.Message = "A change to the Type was made by " + userName;
                newNotification.Created = DateTimeOffset.Now;
                newNotification.UserId = newTicket.AssignToUserId;
                db.Notifications.Add(newNotification);
                db.SaveChanges();
            }

            var newStatus = db.TicketStatuses.Find(newTicket.TicketStatusId);
            var oldStatus = db.TicketStatuses.Find(oldTicket.TicketStatusId);
            if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
            {
                TicketHistory newHistory = new TicketHistory();
                newHistory.AuthorId = currentUser.Id;
                newHistory.TicketId = oldTicket.Id;
                newHistory.Property = "TICKET STATUS UPDATED";
                newHistory.Created = DateTimeOffset.Now;
                if (newStatus != null && oldStatus != null)
                {
                    newHistory.OldValue = oldStatus.Name;
                    newHistory.NewValue = newStatus.Name;
                }               
                db.TicketHistories.Add(newHistory);
                db.SaveChanges();

                Notification newNotification = new Notification();
                newNotification.TicketId = newTicket.Id;
                newNotification.Message = "A change to the Status was made by " + userName;
                newNotification.Created = DateTimeOffset.Now;
                newNotification.UserId = newTicket.AssignToUserId;
                db.Notifications.Add(newNotification);
                db.SaveChanges();
            }

            var newPriority = db.TicketPriorities.Find(newTicket.TicketPriorityId);
            var oldPriority = db.TicketPriorities.Find(oldTicket.TicketPriorityId);
            if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
            {
                TicketHistory newHistory = new TicketHistory();
                newHistory.AuthorId = currentUser.Id;
                newHistory.TicketId = oldTicket.Id;
                newHistory.Property = "TICKET PRIORITY UPDATED";
                newHistory.Created = DateTimeOffset.Now;
                if (newPriority != null && oldPriority != null)
                {
                    newHistory.OldValue = oldPriority.Name;
                    newHistory.NewValue = newPriority.Name;
                }
                
                db.TicketHistories.Add(newHistory);
                db.SaveChanges();

                Notification newNotification = new Notification();
                newNotification.TicketId = newTicket.Id;
                newNotification.Message = "A change to the Priority was made by " + userName;
                newNotification.Created = DateTimeOffset.Now;
                newNotification.UserId = newTicket.AssignToUserId;
                db.Notifications.Add(newNotification);
                db.SaveChanges();
            }
            return;
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Assigned Tickets to user
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult AssignedTickets(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var user = db.Users.Find(User.Identity.GetUserId());    
            var projects = db.Tickets.Where(a => user.Id.Contains(a.AssignToUserId) || user.Id.Contains(a.OwnerUserId)).OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize);
            return View(projects);
        }

        // POST: Create Comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult CommentCreate([Bind(Include = "Id,TicketId,Body,Created,Updated,AuthorId")] TicketComment ticketcomment)
        { // only pass in the bind the attributes that have forms
            var userId = User.Identity.GetUserId();
            var userName = db.Users.Find(User.Identity.GetUserId()).FullName;
            var user = db.Users.Find(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrWhiteSpace(ticketcomment.Body))
                {
                    var ticket = db.Tickets.Find(ticketcomment.TicketId);

                    ticketcomment.Created = System.DateTime.Now;
                    ticketcomment.AuthorId = User.Identity.GetUserId();
                    db.TicketComments.Add(ticketcomment);
                    db.SaveChanges();

                    Notification newNotification = new Notification();
                    newNotification.TicketId = ticket.Id;
                    newNotification.Message = "A comment has been added by " + userName;
                    newNotification.Created = DateTimeOffset.Now;
                    newNotification.UserId = ticket.AssignToUserId;
                    db.Notifications.Add(newNotification);
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = ticket.Id });
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult CreateAttachment([Bind(Include = "Id,TicketId,FileUrl,Created,Description,AuthorId")] TicketAttachment ticketattachment, HttpPostedFileBase image)
        { // only pass in the bind the attributes that have forms
            var userId = User.Identity.GetUserId();
            var userName = db.Users.Find(User.Identity.GetUserId()).FullName;
            var user = db.Users.Find(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                if (image != null && image.ContentLength > 0)
                {
                    var ext = Path.GetExtension(image.FileName).ToLower();
                    if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp" && ext != ".pdf" && ext != ".doc" && ext != ".docx" && ext != ".html")
                        ModelState.AddModelError("image", "Invaild Format.");
                }
                if (!String.IsNullOrWhiteSpace(ticketattachment.Description))
                {
                    var ticket = db.Tickets.Find(ticketattachment.TicketId);

                    if (image != null)
                    {

                        var filepath = "/Assets/img/";
                        var absPath = Server.MapPath("~" + filepath);

                        if (ticketattachment.FileUrl != string.Empty)
                        {
                            ticketattachment.FileUrl = filepath + image.FileName;
                            image.SaveAs(Path.Combine(absPath, image.FileName));
                        }
                    }
                    ticketattachment.Created = System.DateTime.Now;
                    ticketattachment.AuthorId = User.Identity.GetUserId();
                    db.TicketAttachments.Add(ticketattachment);
                    db.SaveChanges();

                    Notification newNotification = new Notification();
                    newNotification.TicketId = ticket.Id;
                    newNotification.Message = "An attachment has been added by " + userName;
                    newNotification.Created = DateTimeOffset.Now;
                    newNotification.UserId = ticket.AssignToUserId; // string type because it is a hash code rather than a regular int Id
                    db.Notifications.Add(newNotification);
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = ticket.Id });
                }
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
