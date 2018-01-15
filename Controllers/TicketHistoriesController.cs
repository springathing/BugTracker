﻿using System;
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

namespace mblyakher_bugtracker.Controllers
{
    [RequireHttps]
    public class TicketHistoriesController : Universal
    {

        // GET: TicketHistories
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Index(int? id)
        {
            Ticket ticket = db.Tickets.Find(id);
            var histories = db.TicketHistories.Where(a => a.TicketId == ticket.Id).OrderByDescending(p => p.Created);
            //var ticketHistories = db.TicketHistories.Include(t => t.Author).Include(t => t.Ticket);
            return View(histories.ToList());
        }

        //// GET: TicketHistories/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketHistory ticketHistory = db.TicketHistories.Find(id);
        //    if (ticketHistory == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ticketHistory);
        //}

        //// GET: TicketHistories/Create
        //public ActionResult Create()
        //{
        //    ViewBag.AuthorId = new SelectList(db.ApplicationUsers, "Id", "FirstName");
        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title");
        //    return View();
        //}

        //// POST: TicketHistories/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,TicketId,Property,OldValue,NewValue,Created,AuthorId")] TicketHistory ticketHistory)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.TicketHistories.Add(ticketHistory);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.AuthorId = new SelectList(db.ApplicationUsers, "Id", "FirstName", ticketHistory.AuthorId);
        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketHistory.TicketId);
        //    return View(ticketHistory);
        //}

        //// GET: TicketHistories/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketHistory ticketHistory = db.TicketHistories.Find(id);
        //    if (ticketHistory == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.AuthorId = new SelectList(db.ApplicationUsers, "Id", "FirstName", ticketHistory.AuthorId);
        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketHistory.TicketId);
        //    return View(ticketHistory);
        //}

        //// POST: TicketHistories/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,TicketId,Property,OldValue,NewValue,Created,AuthorId")] TicketHistory ticketHistory)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(ticketHistory).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.AuthorId = new SelectList(db.ApplicationUsers, "Id", "FirstName", ticketHistory.AuthorId);
        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketHistory.TicketId);
        //    return View(ticketHistory);
        //}

        //// GET: TicketHistories/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketHistory ticketHistory = db.TicketHistories.Find(id);
        //    if (ticketHistory == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ticketHistory);
        //}

        //// POST: TicketHistories/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    TicketHistory ticketHistory = db.TicketHistories.Find(id);
        //    db.TicketHistories.Remove(ticketHistory);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
