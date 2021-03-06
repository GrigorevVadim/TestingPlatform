// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TestingPlatform.Api.Models;

namespace TestingPlatform.Api.Migrations
{
    [DbContext(typeof(ModelsDataContext))]
    [Migration("20210605114125_AddStates")]
    partial class AddStates
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.6");

            modelBuilder.Entity("TestingPlatform.Api.Models.Dal.AnswerDbo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("QuestionId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ResultId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RightAnswer")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserAnswer")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.HasIndex("ResultId");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("TestingPlatform.Api.Models.Dal.QuestionDbo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Answer")
                        .HasColumnType("TEXT");

                    b.Property<string>("Question")
                        .HasColumnType("TEXT");

                    b.Property<int>("State")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("TestId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TestId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("TestingPlatform.Api.Models.Dal.ResultDbo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<double>("Score")
                        .HasColumnType("REAL");

                    b.Property<Guid?>("TestId")
                        .HasColumnType("TEXT");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TestId");

                    b.HasIndex("UserId");

                    b.ToTable("Results");
                });

            modelBuilder.Entity("TestingPlatform.Api.Models.Dal.TestDbo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("State")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Tests");
                });

            modelBuilder.Entity("TestingPlatform.Api.Models.Dal.UserDbo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastLogIn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Login")
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Token")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TestingPlatform.Api.Models.Dal.AnswerDbo", b =>
                {
                    b.HasOne("TestingPlatform.Api.Models.Dal.QuestionDbo", "Question")
                        .WithMany()
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TestingPlatform.Api.Models.Dal.ResultDbo", "Result")
                        .WithMany("Answers")
                        .HasForeignKey("ResultId");

                    b.Navigation("Question");

                    b.Navigation("Result");
                });

            modelBuilder.Entity("TestingPlatform.Api.Models.Dal.QuestionDbo", b =>
                {
                    b.HasOne("TestingPlatform.Api.Models.Dal.TestDbo", "Test")
                        .WithMany("Questions")
                        .HasForeignKey("TestId");

                    b.Navigation("Test");
                });

            modelBuilder.Entity("TestingPlatform.Api.Models.Dal.ResultDbo", b =>
                {
                    b.HasOne("TestingPlatform.Api.Models.Dal.TestDbo", "Test")
                        .WithMany()
                        .HasForeignKey("TestId");

                    b.HasOne("TestingPlatform.Api.Models.Dal.UserDbo", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Test");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TestingPlatform.Api.Models.Dal.TestDbo", b =>
                {
                    b.HasOne("TestingPlatform.Api.Models.Dal.UserDbo", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("TestingPlatform.Api.Models.Dal.ResultDbo", b =>
                {
                    b.Navigation("Answers");
                });

            modelBuilder.Entity("TestingPlatform.Api.Models.Dal.TestDbo", b =>
                {
                    b.Navigation("Questions");
                });
#pragma warning restore 612, 618
        }
    }
}
