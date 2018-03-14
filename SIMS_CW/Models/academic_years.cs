namespace SIMS_CW.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class academic_years
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public academic_years()
        {
            ideas = new HashSet<idea>();
        }

        [Key]
        public int academic_year_id { get; set; }

        [StringLength(50)]
        public string academic_year_name { get; set; }

        public DateTime? started_at { get; set; }

        public DateTime? ended_at { get; set; }

        public DateTime? deadline_ideas { get; set; }

        public DateTime? deadline_comments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<idea> ideas { get; set; }
    }
}
