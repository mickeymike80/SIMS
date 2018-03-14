namespace SIMS_CW.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class rate
    {
        [Key]
        public int rate_id { get; set; }

        public int? user_id { get; set; }

        public int? idea_id { get; set; }

        public int? rate_point { get; set; }

        public DateTime? created_at { get; set; }

        public virtual idea idea { get; set; }

        public virtual user user { get; set; }
    }
}
