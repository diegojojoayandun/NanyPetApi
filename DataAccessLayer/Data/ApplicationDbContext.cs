using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public partial class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Appointment> Appointments { get; set; } = null!;
        public virtual DbSet<Herder> Herders { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Owner> Owners { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Pet> Pets { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual new DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) { }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Herder>(entity =>
            {
                entity.ToTable("herders");
                entity.HasIndex(e => e.EmailUser, "ix_herders_email_user");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.EmailUser).HasColumnName("email_user");
                entity.Property(e => e.Phone).HasMaxLength(30).HasColumnName("phone");
                entity.Property(e => e.Address).HasMaxLength(100).HasColumnName("address");
                entity.Property(e => e.City).HasMaxLength(30).HasColumnName("city");
                entity.Property(e => e.State).HasMaxLength(30).HasColumnName("state");
                entity.Property(e => e.Location).HasMaxLength(100).HasColumnName("location");
                entity.Property(e => e.HourlyRate).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AverageRating).HasDefaultValue(0.0);
                entity.Property(e => e.TotalReviews).HasDefaultValue(0);

                entity.HasOne(d => d.UserNameNavigation)
                    .WithMany(p => p.Herders)
                    .HasPrincipalKey(p => p.UserName)
                    .HasForeignKey(d => d.EmailUser)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_herder_user");
            });

            modelBuilder.Entity<Owner>(entity =>
            {
                entity.ToTable("owners");
                entity.HasIndex(e => e.EmailUser, "ix_owners_email_user").IsUnique();
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.EmailUser).HasMaxLength(256).HasColumnName("email_user");
                entity.Property(e => e.Phone).HasMaxLength(30).HasColumnName("phone");
                entity.Property(e => e.Address).HasMaxLength(100).HasColumnName("address");
                entity.Property(e => e.City).HasMaxLength(30).HasColumnName("city");
                entity.Property(e => e.State).HasMaxLength(30).HasColumnName("state");
                entity.Property(e => e.Location).HasMaxLength(100).HasColumnName("location");

                entity.HasOne(d => d.UserNameNavigation)
                    .WithOne(p => p.Owner)
                    .HasPrincipalKey<User>(p => p.Email)
                    .HasForeignKey<Owner>(d => d.EmailUser)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_owner_user");
            });

            modelBuilder.Entity<Pet>(entity =>
            {
                entity.ToTable("pets");
                entity.HasIndex(e => e.OwnerId, "ix_pets_owner_id");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasMaxLength(60).HasColumnName("name");
                entity.Property(e => e.Species).HasMaxLength(30).HasColumnName("species");
                entity.Property(e => e.Breed).HasMaxLength(30).HasColumnName("breed");
                entity.Property(e => e.Age).HasColumnName("age");
                entity.Property(e => e.Gender).HasMaxLength(20).HasColumnName("gender");
                entity.Property(e => e.OwnerId).HasColumnName("owner_id");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Pets)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_pet_owner");
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("appointments");
                entity.HasIndex(e => e.HerderId, "ix_appointments_herder_id");
                entity.HasIndex(e => e.PetId, "ix_appointments_pet_id");
                entity.HasIndex(e => e.Status, "ix_appointments_status");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.PetId).HasColumnName("pet_id");
                entity.Property(e => e.HerderId).HasColumnName("herder_id");
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Notes).HasColumnName("notes");
                entity.Property(e => e.SpecialRequirements);

                entity.HasOne(d => d.Herder)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.HerderId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_appointment_herder");

                entity.HasOne(d => d.Pet)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.PetId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_appointment_pet");

                entity.HasOne(d => d.Payment)
                    .WithOne(p => p.Appointment)
                    .HasForeignKey<Payment>(p => p.AppointmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_payment_appointment");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("payments");
                entity.HasIndex(e => e.AppointmentId, "ix_payments_appointment_id").IsUnique();
                entity.HasIndex(e => e.WompiReference, "ix_payments_wompi_reference");
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Currency).HasMaxLength(3).HasDefaultValue("COP");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("reviews");
                entity.HasIndex(e => e.AppointmentId, "ix_reviews_appointment_id");
                entity.HasIndex(e => e.ReviewedId, "ix_reviews_reviewed_id");
                // Un reviewer solo puede dejar una reseña por cita por tipo
                entity.HasIndex(e => new { e.AppointmentId, e.ReviewerId, e.Type }, "uq_review_appointment_reviewer_type").IsUnique();

                entity.HasOne(d => d.Appointment)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.AppointmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_review_appointment");

                entity.HasOne(d => d.Reviewer)
                    .WithMany()
                    .HasForeignKey(d => d.ReviewerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_review_reviewer");

                entity.HasOne(d => d.Reviewed)
                    .WithMany()
                    .HasForeignKey(d => d.ReviewedId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_review_reviewed");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("messages");
                entity.HasIndex(e => e.AppointmentId, "ix_messages_appointment_id");
                entity.HasIndex(e => e.SenderId, "ix_messages_sender_id");
                entity.Property(e => e.Content);

                entity.HasOne(d => d.Appointment)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.AppointmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_message_appointment");

                entity.HasOne(d => d.Sender)
                    .WithMany()
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_message_sender");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notifications");
                entity.HasIndex(e => e.UserId, "ix_notifications_user_id");
                entity.HasIndex(e => new { e.UserId, e.IsRead }, "ix_notifications_user_unread");
                entity.Property(e => e.Title).HasMaxLength(200);
                entity.Property(e => e.Body);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_notification_user");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
