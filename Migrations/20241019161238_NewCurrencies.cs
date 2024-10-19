using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoCalculator.Migrations
{
    /// <inheritdoc />
    public partial class NewCurrencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.UpdateData(
                table: "CryptoCurrencies",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Name", "Symbol" },
                values: new object[] { "Tether", "USDT" });

            migrationBuilder.UpdateData(
                table: "CryptoCurrencies",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "Symbol" },
                values: new object[] { "BNB", "BNB" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.UpdateData(
                table: "CryptoCurrencies",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Name", "Symbol" },
                values: new object[] { "Solana", "SOL" });

            migrationBuilder.UpdateData(
                table: "CryptoCurrencies",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "Symbol" },
                values: new object[] { "Ripple", "XRP" });

        }
    }
}
