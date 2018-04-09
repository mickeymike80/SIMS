namespace SIMS_CW.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class user
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public user()
        {
            comments = new HashSet<comment>();
            ideas = new HashSet<idea>();
            rates = new HashSet<rate>();
        }

        [Key]
        public int user_id { get; set; }

        public int? user_university_id { get; set; }

        public int? department_id { get; set; }

        public int? role_id { get; set; }

        [StringLength(50)]
        public string user_name { get; set; }

        [StringLength(100)]
        public string email { get; set; }

        [StringLength(50)]
        public string password { get; set; }

        public DateTime? created_at { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<comment> comments { get; set; }

        public virtual department department { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<idea> ideas { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<rate> rates { get; set; }

        public virtual role role { get; set; }

        public override string ToString()
        {
            return user_name;
        }
    }
}
