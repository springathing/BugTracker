using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mblyakher_bugtracker.Models.CodeFirst
{
    public class TicketAttachment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public string AuthorId { get; set; }
        public string FileUrl { get; set; }
        // filepath was to be used for file deletion, it was going to give you the full path to the image, 
        // can use mappath method in control so you don't need it

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser Author { get; set; }
    }
}