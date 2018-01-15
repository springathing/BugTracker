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
using System.IO;

namespace mblyakher_bugtracker.Controllers
{
    [RequireHttps]
    public class TicketAttachmentsController : Universal
    {
        //// GET: TicketAttachments
        //public ActionResult Index()
        //{
        //    var ticketAttachments = db.TicketAttachments.Include(t => t.Author).Include(t => t.Ticket);
        //    return View(ticketAttachments.ToList());
        //}

        //// GET: TicketAttachments/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
        //    if (ticketAttachment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ticketAttachment);
        //}

        //// GET: TicketAttachments/Create
        //public ActionResult Create()
        //{
        //    ViewBag.AuthorId = new SelectList(db.ApplicationUsers, "Id", "FirstName");
        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title");
        //    return View();
        //}

        //// POST: TicketAttachments/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,TicketId,Description,Created,AuthorId,FileUrl")] TicketAttachment ticketAttachment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.TicketAttachments.Add(ticketAttachment);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.AuthorId = new SelectList(db.ApplicationUsers, "Id", "FirstName", ticketAttachment.AuthorId);
        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachment.TicketId);
        //    return View(ticketAttachment);
        //}

        // GET: TicketAttachments/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
        //    if (ticketAttachment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    //ViewBag.AuthorId = new SelectList(db.ApplicationUsers, "Id", "FirstName", ticketAttachment.AuthorId);
        //    //ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachment.TicketId);
        //    return View(ticketAttachment);
        //}

        //// POST: TicketAttachments/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,TicketId,Description,Created,AuthorId,FileUrl")] TicketAttachment ticketAttachment, HttpPostedFileBase image)
        //{
        //    if (image != null && image.ContentLength > 0)
        //    {
        //        var ext = Path.GetExtension(image.FileName).ToLower();
        //        if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp") // curly braces not needed for 1 line if statements
        //        {
        //            ModelState.AddModelError("image", "Invalid Format.");
        //        }
        //    }

        //        if (image != null)
        //        {
        //            var filePath = "/Assets/img/";
        //            var absPath = Server.MapPath("~" + filePath);
        //            ticketAttachment.FileUrl = filePath + image.FileName;
        //            image.SaveAs(Path.Combine(absPath, image.FileName));
        //        }

        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(ticketAttachment).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
        //    }
        //    //ViewBag.AuthorId = new SelectList(db.ApplicationUsers, "Id", "FirstName", ticketAttachment.AuthorId);
        //    //ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachment.TicketId);
        //    return View(ticketAttachment);
        //}

        //// GET: TicketAttachments/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
        //    if (ticketAttachment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ticketAttachment);
        //}

        // POST: TicketAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            db.TicketAttachments.Remove(ticketAttachment);
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
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
