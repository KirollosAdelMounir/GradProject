﻿// <auto-generated />
using System;
using HealthCareSysAPI.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HealthCareSysAPI.Migrations
{
    [DbContext(typeof(HealthCareSysDBContext))]
    partial class HealthCareSysDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("HealthCareSys.Models.Appointment", b =>
                {
                    b.Property<int>("AppointmentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AppointmentID"), 1L, 1);

                    b.Property<DateTime>("AppointmentDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("AppointmentRating")
                        .HasColumnType("int");

                    b.Property<string>("DoctorID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsDone")
                        .HasColumnType("bit");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("AppointmentID");

                    b.HasIndex("DoctorID");

                    b.HasIndex("UserId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("HealthCareSys.Models.Comment", b =>
                {
                    b.Property<int>("CommentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CommentID"), 1L, 1);

                    b.Property<DateTime>("CommentDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("CommentRating")
                        .HasColumnType("int");

                    b.Property<string>("CommentText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DoctorID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ForumID")
                        .HasColumnType("int");

                    b.HasKey("CommentID");

                    b.HasIndex("DoctorID");

                    b.HasIndex("ForumID");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("HealthCareSys.Models.Doctor", b =>
                {
                    b.Property<string>("DoctorID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<float>("AverageRating")
                        .HasColumnType("real");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("specializationSpecID")
                        .HasColumnType("int");

                    b.HasKey("DoctorID");

                    b.HasIndex("UserID");

                    b.HasIndex("specializationSpecID");

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("HealthCareSys.Models.Forum", b =>
                {
                    b.Property<int>("PostID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PostID"), 1L, 1);

                    b.Property<DateTime>("DateOfPost")
                        .HasColumnType("datetime2");

                    b.Property<string>("PostText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("specializationSpecID")
                        .HasColumnType("int");

                    b.HasKey("PostID");

                    b.HasIndex("UserID");

                    b.HasIndex("specializationSpecID");

                    b.ToTable("Forums");
                });

            modelBuilder.Entity("HealthCareSys.Models.MedicalHistory", b =>
                {
                    b.Property<int>("MedicalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MedicalId"), 1L, 1);

                    b.Property<DateTime>("DateOfInfection")
                        .HasColumnType("datetime2");

                    b.Property<string>("Disease")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RelationShip")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("MedicalId");

                    b.HasIndex("UserID");

                    b.ToTable("MedicalHistories");
                });

            modelBuilder.Entity("HealthCareSys.Models.Specialization", b =>
                {
                    b.Property<int>("SpecID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SpecID"), 1L, 1);

                    b.Property<int>("NumberOfDoctors")
                        .HasColumnType("int");

                    b.Property<string>("SpecializationDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SpecializationName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SpecID");

                    b.ToTable("Specializations");
                });

            modelBuilder.Entity("HealthCareSysAPI.Models.Favorite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("DoctorID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("DoctorID");

                    b.HasIndex("UserID");

                    b.ToTable("Favorites");
                });

            modelBuilder.Entity("HealthCareSysAPI.Models.HealthCareSysUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Blood")
                        .HasColumnType("int");

                    b.Property<bool>("ConfirmEmail")
                        .HasColumnType("bit");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("Gender")
                        .HasColumnType("bit");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("HealthCareSysAPI.Models.ScheduleTiming", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("DoctorID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("TimeFrom")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("TimeTo")
                        .HasColumnType("datetime2");

                    b.Property<int>("day")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DoctorID");

                    b.ToTable("ScheduleTimings");
                });

            modelBuilder.Entity("HealthCareSys.Models.Appointment", b =>
                {
                    b.HasOne("HealthCareSys.Models.Doctor", "doctor")
                        .WithMany()
                        .HasForeignKey("DoctorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HealthCareSysAPI.Models.HealthCareSysUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("doctor");
                });

            modelBuilder.Entity("HealthCareSys.Models.Comment", b =>
                {
                    b.HasOne("HealthCareSys.Models.Doctor", "doctor")
                        .WithMany()
                        .HasForeignKey("DoctorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HealthCareSys.Models.Forum", "forum")
                        .WithMany()
                        .HasForeignKey("ForumID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("doctor");

                    b.Navigation("forum");
                });

            modelBuilder.Entity("HealthCareSys.Models.Doctor", b =>
                {
                    b.HasOne("HealthCareSysAPI.Models.HealthCareSysUser", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HealthCareSys.Models.Specialization", "specialization")
                        .WithMany()
                        .HasForeignKey("specializationSpecID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("specialization");
                });

            modelBuilder.Entity("HealthCareSys.Models.Forum", b =>
                {
                    b.HasOne("HealthCareSysAPI.Models.HealthCareSysUser", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HealthCareSys.Models.Specialization", "Specialization")
                        .WithMany()
                        .HasForeignKey("specializationSpecID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Specialization");

                    b.Navigation("User");
                });

            modelBuilder.Entity("HealthCareSys.Models.MedicalHistory", b =>
                {
                    b.HasOne("HealthCareSysAPI.Models.HealthCareSysUser", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("HealthCareSysAPI.Models.Favorite", b =>
                {
                    b.HasOne("HealthCareSys.Models.Doctor", "doctor")
                        .WithMany()
                        .HasForeignKey("DoctorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HealthCareSysAPI.Models.HealthCareSysUser", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("doctor");
                });

            modelBuilder.Entity("HealthCareSysAPI.Models.ScheduleTiming", b =>
                {
                    b.HasOne("HealthCareSys.Models.Doctor", "doctor")
                        .WithMany()
                        .HasForeignKey("DoctorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("doctor");
                });
#pragma warning restore 612, 618
        }
    }
}
