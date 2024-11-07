using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoCalculator.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CryptoCurrencies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Symbol = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoCurrencies", x => x.Id);
                });

            //migrationBuilder.CreateTable(
            //    name: "DCAInvestments",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CryptoCurrencyId = table.Column<int>(nullable: false),
            //        InvestmentDate = table.Column<DateTime>(nullable: false),
            //        InvestedAmount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
            //        CryptoAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        ValueToday = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
            //        ROI = table.Column<decimal>(type: "decimal(18, 2)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_DCAInvestments", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_DCAInvestments_CryptoCurrencies_CryptoCurrencyId",
            //            column: x => x.CryptoCurrencyId,
            //            principalTable: "CryptoCurrencies",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateTable(
                name: "PriceData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CryptoCurrencyId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceData_CryptoCurrencies_CryptoCurrencyId",
                        column: x => x.CryptoCurrencyId,
                        principalTable: "CryptoCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Seed data
            migrationBuilder.InsertData(
                table: "CryptoCurrencies",
                columns: new[] { "Id", "Name", "Symbol" },
                values: new object[,]
                {
                    { 1, "Bitcoin", "BTC" },
                    { 2, "Ethereum", "ETH" },
                    { 3, "Solana", "SOL" },
                    { 4, "Ripple", "XRP" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceData");

            migrationBuilder.DropTable(
                name: "DCAInvestments");

            migrationBuilder.DropTable(
                name: "CryptoCurrencies");
        }
    }
}