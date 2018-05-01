using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIMS_CW.Models
{
    public class display_idea
    {
        public user user { get; set; }
        public idea idea { get; set; }
        public int rate_point { get; set; }
        public int rate_point_up { get; set; }
        public int rate_point_down { get; set; }
        public int number_of_comments { get; set; }
    }
}