using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscountService.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixUserIdToDiscountUsage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "DiscountUsages");

            migrationBuilder.Sql("DELETE FROM \"DiscountUsages\"");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DiscountUsages");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "DiscountUsages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UsedAt",
                table: "DiscountUsages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 16, 8, 28, 55, 563, DateTimeKind.Utc).AddTicks(6239),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 5, 25, 13, 51, 29, 995, DateTimeKind.Utc).AddTicks(1754));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: true,
                defaultValue: new DateTime(2026, 6, 16, 8, 28, 55, 563, DateTimeKind.Utc).AddTicks(4823),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValue: new DateTime(2026, 5, 25, 13, 51, 29, 995, DateTimeKind.Utc).AddTicks(529));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 16, 8, 28, 55, 563, DateTimeKind.Utc).AddTicks(4558),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 5, 25, 13, 51, 29, 995, DateTimeKind.Utc).AddTicks(297));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "DiscountUsages",
                type: "integer",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UsedAt",
                table: "DiscountUsages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 25, 13, 51, 29, 995, DateTimeKind.Utc).AddTicks(1754),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 6, 16, 8, 28, 55, 563, DateTimeKind.Utc).AddTicks(6239));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "DiscountUsages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: true,
                defaultValue: new DateTime(2026, 5, 25, 13, 51, 29, 995, DateTimeKind.Utc).AddTicks(529),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValue: new DateTime(2026, 6, 16, 8, 28, 55, 563, DateTimeKind.Utc).AddTicks(4823));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 25, 13, 51, 29, 995, DateTimeKind.Utc).AddTicks(297),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 6, 16, 8, 28, 55, 563, DateTimeKind.Utc).AddTicks(4558));
        }
    }
}
