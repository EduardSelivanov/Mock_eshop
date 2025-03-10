﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SlotsService.Data;

#nullable disable

namespace SlotsService.Migrations
{
    [DbContext(typeof(SlotsContext))]
    [Migration("20250218133552_editedSlot")]
    partial class editedSlot
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SlotsService.Models.SlotModel", b =>
                {
                    b.Property<Guid>("SlotId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateOnly?>("ArriveDate")
                        .HasColumnType("date");

                    b.Property<bool>("IsFree")
                        .HasColumnType("bit");

                    b.Property<Guid>("OnRackId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SKU")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("X")
                        .HasColumnType("int");

                    b.Property<int>("Y")
                        .HasColumnType("int");

                    b.HasKey("SlotId");

                    b.ToTable("SlotsTable");
                });
#pragma warning restore 612, 618
        }
    }
}
