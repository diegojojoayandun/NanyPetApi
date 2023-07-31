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

        //public virtual DbSet<Appointment> Appointments { get; set; } = null!;
        public virtual DbSet<Herder> Herders { get; set; } = null!;
        public virtual DbSet<Owner> Owners { get; set; } = null!;
        //public virtual DbSet<Pet> Pets { get; set; } = null!;
        public virtual new DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("appointments");

                entity.HasIndex(e => e.HerderId, "fk_appointment_herder");

                entity.HasIndex(e => e.PetId, "fk_appointment_pet");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.AnimalId).HasColumnName("animal_id");

                entity.Property(e => e.AppointmentTime)
                    .HasColumnType("timestamp")
                    .HasColumnName("appointment_time");

                entity.Property(e => e.HerderId).HasColumnName("herder_id");

                entity.Property(e => e.Notes)
                    .HasColumnType("text")
                    .HasColumnName("notes");

                entity.Property(e => e.PetId).HasColumnName("pet_id");

                entity.HasOne(d => d.Herder)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.HerderId)
                    .HasConstraintName("fk_appointment_herder");

                entity.HasOne(d => d.Pet)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.PetId)
                    .HasConstraintName("fk_appointment_pet");
            });

            modelBuilder.Entity<Herder>(entity =>
            {
                entity.ToTable("herders");

                entity.HasIndex(e => e.EmailUser, "email_user");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(100)
                    .HasColumnName("address");

                entity.Property(e => e.City)
                    .HasMaxLength(30)
                    .HasColumnName("city");

                entity.Property(e => e.EmailUser)
                    .HasColumnName("email_user");

                entity.Property(e => e.Location)
                    .HasMaxLength(100)
                    .HasColumnName("location");

                entity.Property(e => e.Phone)
                    .HasMaxLength(30)
                    .HasColumnName("phone");

                entity.Property(e => e.State)
                    .HasMaxLength(30)
                    .HasColumnName("state");

                entity.HasOne(d => d.UserNameNavigation)
                    .WithMany(p => p.Herders)
                    .HasPrincipalKey(p => p.UserName)
                    .HasForeignKey(d => d.EmailUser)
                    .HasConstraintName("herders_ibfk_1");
            });

            modelBuilder.Entity<Owner>(entity =>
            {
                entity.ToTable("owners");

                entity.HasIndex(e => e.EmailUser, "email_user")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(100)
                    .HasColumnName("address");

                entity.Property(e => e.City)
                    .HasMaxLength(30)
                    .HasColumnName("city");

                entity.Property(e => e.EmailUser)
                    .HasMaxLength(256)
                    .HasColumnName("email_user");


                entity.Property(e => e.Location)
                    .HasMaxLength(100)
                    .HasColumnName("location");

                entity.Property(e => e.Phone)
                    .HasMaxLength(30)
                    .HasColumnName("phone");

                entity.Property(e => e.State)
                    .HasMaxLength(30)
                    .HasColumnName("state");

                entity.HasOne(d => d.UserNameNavigation)
                    .WithOne(p => p.Owner)
                    .HasPrincipalKey<User>(p => p.Email)
                    .HasForeignKey<Owner>(d => d.EmailUser)
                    .HasConstraintName("owners_ibfk_1");
            });

            //modelBuilder.Entity<Pet>(entity =>
            //{
            //    entity.ToTable("pets");

            //    entity.HasIndex(e => e.OwnerId, "fk_pet_owner");

            //    entity.Property(e => e.Id).HasColumnName("id");

            //    entity.Property(e => e.Age).HasColumnName("age");

            //    entity.Property(e => e.Breed)
            //        .HasMaxLength(30)
            //        .HasColumnName("breed");

            //    entity.Property(e => e.Gender)
            //        .HasMaxLength(20)
            //        .HasColumnName("gender");

            //    entity.Property(e => e.Name)
            //        .HasMaxLength(60)
            //        .HasColumnName("name");

            //    entity.Property(e => e.OwnerId).HasColumnName("owner_id");

            //    entity.Property(e => e.Species)
            //        .HasMaxLength(30)
            //        .HasColumnName("species");

            //    entity.HasOne(d => d.Owner)
            //        .WithMany(p => p.Pets)
            //        .HasForeignKey(d => d.OwnerId)
            //        .HasConstraintName("fk_pet_owner");
            //});


            //modelBuilder.Entity<User>(entity =>
            //{
            //    entity.ToTable("users");

            //    entity.HasIndex(e => e.Email, "email")
            //        .IsUnique();

            //    entity.Property(e => e.Id).HasColumnName("id");

            //    entity.Property(e => e.Email)
            //        .HasMaxLength(60)
            //        .HasColumnName("email");

            //    entity.Property(e => e.Password)
            //        .HasMaxLength(60)
            //        .HasColumnName("password");


            //});

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
