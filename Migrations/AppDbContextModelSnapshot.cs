﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CryptoCalculator.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CryptoCalculator.Models.CryptoCurrency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("CryptoCurrencies");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Bitcoin",
                            Symbol = "BTC"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Ethereum",
                            Symbol = "ETH"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Tether",
                            Symbol = "USDT"
                        },
                        new
                        {
                            Id = 4,
                            Name = "BNB",
                            Symbol = "BNB"
                        });
                });

            modelBuilder.Entity("CryptoCalculator.Models.PriceData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CryptoCurrencyId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Price")
                        .HasPrecision(18, 8)
                        .HasColumnType("decimal(18,8)");

                    b.HasKey("Id");

                    b.HasIndex("CryptoCurrencyId");

                    b.ToTable("PriceData");
                });

            modelBuilder.Entity("CryptoCalculator.Models.PriceData", b =>
                {
                    b.HasOne("CryptoCalculator.Models.CryptoCurrency", "CryptoCurrency")
                        .WithMany()
                        .HasForeignKey("CryptoCurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CryptoCurrency");
                });
#pragma warning restore 612, 618
        }
    }
}
