using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mblyakher_bugtracker.Models.CodeFirst
{
    public class TicketStatus // sometimes models are created just for holding information (LookUp Tables)
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}