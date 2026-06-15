using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscountService.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropProductIdToDiscountUsage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "DiscountUsages");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UsedAt",
                table: "DiscountUsages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 14, 9, 5, 13, 989, DateTimeKind.Utc).AddTicks(6293),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 5, 25, 13, 51, 29, 995, DateTimeKind.Utc).AddTicks(1754));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: true,
                defaultValue: new DateTime(2026, 6, 14, 9, 5, 13, 989, DateTimeKind.Utc).AddTicks(4987),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValue: new DateTime(2026, 5, 25, 13, 51, 29, 995, DateTimeKind.Utc).AddTicks(529));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 14, 9, 5, 13, 989, DateTimeKind.Utc).AddTicks(4728),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 5, 25, 13, 51, 29, 995, DateTimeKind.Utc).AddTicks(297));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UsedAt",
                table: "DiscountUsages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 25, 13, 51, 29, 995, DateTimeKind.Utc).AddTicks(1754),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 6, 14, 9, 5, 13, 989, DateTimeKind.Utc).AddTicks(6293));

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
                oldDefaultValue: new DateTime(2026, 6, 14, 9, 5, 13, 989, DateTimeKind.Utc).AddTicks(4987));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 25, 13, 51, 29, 995, DateTimeKind.Utc).AddTicks(297),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 6, 14, 9, 5, 13, 989, DateTimeKind.Utc).AddTicks(4728));
        }
    }
}
