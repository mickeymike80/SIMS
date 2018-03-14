namespace SIMS_CW.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class document
    {
        [Key]
        public int document_id { get; set; }

        public int? idea_id { get; set; }

        [StringLength(100)]
        public string old_file_name { get; set; }

        [StringLength(100)]
        public string new_file_name { get; set; }

        public DateTime? created_at { get; set; }

        public virtual idea idea { get; set; }
    }
}
