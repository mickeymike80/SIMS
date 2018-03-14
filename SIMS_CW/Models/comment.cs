namespace SIMS_CW.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class comment
    {
        [Key]
        public int comment_id { get; set; }

        public int? user_id { get; set; }

        public int? idea_id { get; set; }

        [Column(TypeName = "text")]
        public string comment_content { get; set; }

        public byte? isAnonymous { get; set; }

        public byte? isEnabled { get; set; }

        public byte? status { get; set; }

        public DateTime? created_at { get; set; }

        public virtual idea idea { get; set; }

        public virtual user user { get; set; }
    }
}
