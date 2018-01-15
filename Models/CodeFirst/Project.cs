using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mblyakher_bugtracker.Models.CodeFirst
{
    public class Project
    {
        public Project()
        {
            Users = new HashSet<ApplicationUser>();
            Tickets = new HashSet<Ticket>();
            this.IsActive = true;
        }
        public int? Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AuthorId { get; set; }
        public bool IsActive { get; set; }
        public string AssignedToId { get; set; } // Project Manager I'm assigning

        // we don't have public virtual ApplicationUser Author { get; set; } because it would be pointing to the same table / it would be trying to create two different types of relationships in the same table
        public virtual ICollection<ApplicationUser> Users { get; set; } // These are the assigned Users
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ApplicationUser AssignedTo { get; set; }
    }
}