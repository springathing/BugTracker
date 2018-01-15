using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mblyakher_bugtracker.Models.CodeFirst
{
    public class TicketType // will start out by hard coding these values in the tables, can you allow user or admin to changes these later
    { // sometimes models are created just for holding information (LookUp Tables)
        public int Id { get; set; }
        public string Name { get; set; }
    }
}