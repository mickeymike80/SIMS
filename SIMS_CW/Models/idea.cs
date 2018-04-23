namespace SIMS_CW.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class idea
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public idea()
        {
            comments = new HashSet<comment>();
            documents = new HashSet<document>();
            rates = new HashSet<rate>();
        }

        [Key]
        public int idea_id { get; set; }

        public int? user_id { get; set; }

        public int? category_id { get; set; }

        public int? academic_year_id { get; set; }

        [StringLength(50)]
        public string idea_title { get; set; }

        [Column(TypeName = "text")]
        [Required(ErrorMessage = "Content Cannot be Blank")]
        public string idea_content { get; set; }

        public int? viewed_count { get; set; }

        public byte? isAnonymous { get; set; }

        public byte? isEnabled { get; set; }

        public byte? status { get; set; }

        public DateTime? created_at { get; set; }

        public virtual academic_years academic_years { get; set; }

        public virtual category category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<comment> comments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<document> documents { get; set; }

        public virtual user user { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<rate> rates { get; set; }
    }
}
