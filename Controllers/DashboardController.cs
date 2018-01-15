using mblyakher_bugtracker.Models.CodeFirst;
using mblyakher_bugtracker.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static mblyakher_bugtracker.EmailService;

namespace mblyakher_bugtracker.Controllers
{
    [RequireHttps]
    public class DashboardController : Universal
    {
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Index(string searchStr)
        {
            ViewBag.Search = searchStr;
            var projectTickets = IndexSearch(searchStr);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public IQueryable<Project> IndexSearch(string searchStr)
        {
            IQueryable<Project> result = null;
            if (searchStr != null)
            {
                result = db.Projects.AsQueryable();
                result = result.Where(p => p.Title.Contains(searchStr) ||
                p.Description.Contains(searchStr) ||
                p.AuthorId.Contains(searchStr) ||
                p.AssignedToId.Contains(searchStr) ||
                p.Tickets.Any(t => t.Title.Contains(searchStr) ||
                t.Description.Contains(searchStr)));
                //return View(result.OrderByDescending(p => p.Created));
            }
            else
            {
                result = db.Projects.AsQueryable();
            }
            return result.OrderByDescending(p => p.Created);
        }

        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Contact()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold> ({1})</p><p> Message:</p><p>{2}</p>";
                    var from = model.FromName + "<" + model.FromEmail + ">"; //MyPortfolio part is the name that shows up on email preview
                    //model.Body = "This is a message from your portfolio site. The name and the email of the contacting person is above.";

                    var email = new MailMessage(from, ConfigurationManager.AppSettings["emailto"])
                    {           //composing body here,
                        Subject = "BugTracker Contact",
                        Body = string.Format(body, model.FromName, model.FromEmail,
                        model.Body),
                        IsBodyHtml = true
                    };

                    var svc = new PersonalEmail(); //instantiation to be able to access SendAsync method(it actually sends the email)
                    await svc.SendAsync(email);

                    return RedirectToAction("Contact");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model); //return to view with empty email model
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