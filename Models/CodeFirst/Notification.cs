using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace mblyakher_bugtracker.Models.CodeFirst
{
    public class Notification
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        [Required]
        public string Message { get; set; }
        public DateTimeOffset Created { get; set; }
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}