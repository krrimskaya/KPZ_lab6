using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NotesApp.Models;

namespace NotesApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Note> Notes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<NoteTag> NoteTags { get; set; }
        public DbSet<NoteHistory> NoteHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Налаштування зв'язку багато-до-багатьох
            modelBuilder.Entity<NoteTag>()
                .HasKey(nt => new { nt.NoteId, nt.TagId });

            modelBuilder.Entity<NoteTag>()
                .HasOne(nt => nt.Note)
                .WithMany(n => n.NoteTags)
                .HasForeignKey(nt => nt.NoteId);

            modelBuilder.Entity<NoteTag>()
                .HasOne(nt => nt.Tag)
                .WithMany(t => t.NoteTags)
                .HasForeignKey(nt => nt.TagId);

            modelBuilder.Entity<Tag>()
                .Property(t => t.UserId)
                .IsRequired(false);

            // Налаштування для історії нотаток
            modelBuilder.Entity<NoteHistory>()
                .HasOne(nh => nh.Note)
                .WithMany()
                .HasForeignKey(nh => nh.NoteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Додавання стандартних тегів
            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Name = "Робота", Color = "#0d6efd", UserId = null },
                new Tag { Id = 2, Name = "Навчання", Color = "#198754", UserId = null },
                new Tag { Id = 3, Name = "Спорт", Color = "#dc3545", UserId = null },
                new Tag { Id = 4, Name = "Особисте", Color = "#6f42c1", UserId = null },
                new Tag { Id = 5, Name = "Здоров'я", Color = "#fd7e14", UserId = null },
                new Tag { Id = 6, Name = "Фінанси", Color = "#20c997", UserId = null }
            );
        }
    }
}