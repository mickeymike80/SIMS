using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS_CW.Models
{
    public class unviewed_idea
    {
        public user user { get; set; }

        public idea idea { get; set; }

        public Boolean viewed { get; set; }
    }
}