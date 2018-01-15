using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mblyakher_bugtracker.Models.CodeFirst
{
    public class TicketPriority // sometimes models are created just for holding information (LookUp Tables)
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}