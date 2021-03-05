﻿// <auto-generated />
using System;
using DbRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DbRepository.Migrations
{
    [DbContext(typeof(UsersRepositoryContext))]
    [Migration("20210225100101_UserRolesFix2")]
    partial class UserRolesFix2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DbModels.RoleGroupModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("GroupName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RoleGroups");
                });

            modelBuilder.Entity("DbModels.RoleModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RoleGroupId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleGroupId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("DbModels.UserModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Login")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DbModels.UsersRoleRelation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("RoleId")
                        .HasColumnType("int");

                    b.Property<int?>("UserModelId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserModelId");

                    b.ToTable("UsersRoleRelation");
                });

            modelBuilder.Entity("DbModels.RoleModel", b =>
                {
                    b.HasOne("DbModels.RoleGroupModel", "RoleGroup")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleGroupId");

                    b.Navigation("RoleGroup");
                });

            modelBuilder.Entity("DbModels.UsersRoleRelation", b =>
                {
                    b.HasOne("DbModels.RoleModel", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("DbModels.UserModel", null)
                        .WithMany("Roles")
                        .HasForeignKey("UserModelId");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("DbModels.RoleGroupModel", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("DbModels.UserModel", b =>
                {
                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}
