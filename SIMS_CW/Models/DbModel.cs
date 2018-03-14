namespace SIMS_CW.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DbModel : DbContext
    {
        public DbModel()
            : base("name=DbModel")
        {
        }

        public virtual DbSet<academic_years> academic_years { get; set; }
        public virtual DbSet<category> categories { get; set; }
        public virtual DbSet<comment> comments { get; set; }
        public virtual DbSet<department> departments { get; set; }
        public virtual DbSet<document> documents { get; set; }
        public virtual DbSet<idea> ideas { get; set; }
        public virtual DbSet<rate> rates { get; set; }
        public virtual DbSet<role> roles { get; set; }
        public virtual DbSet<user> users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<academic_years>()
                .Property(e => e.academic_year_name)
                .IsUnicode(false);

            modelBuilder.Entity<category>()
                .Property(e => e.category_name)
                .IsUnicode(false);

            modelBuilder.Entity<comment>()
                .Property(e => e.comment_content)
                .IsUnicode(false);

            modelBuilder.Entity<department>()
                .Property(e => e.department_name)
                .IsUnicode(false);

            modelBuilder.Entity<document>()
                .Property(e => e.old_file_name)
                .IsUnicode(false);

            modelBuilder.Entity<document>()
                .Property(e => e.new_file_name)
                .IsUnicode(false);

            modelBuilder.Entity<idea>()
                .Property(e => e.idea_title)
                .IsUnicode(false);

            modelBuilder.Entity<idea>()
                .Property(e => e.idea_content)
                .IsUnicode(false);

            modelBuilder.Entity<role>()
                .Property(e => e.role_name)
                .IsUnicode(false);

            modelBuilder.Entity<role>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<user>()
                .Property(e => e.user_name)
                .IsUnicode(false);

            modelBuilder.Entity<user>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<user>()
                .Property(e => e.password)
                .IsUnicode(false);
        }
    }
}
