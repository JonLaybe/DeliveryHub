using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscountService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductIdToDiscountUsage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UsedAt",
                table: "DiscountUsages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 25, 13, 47, 54, 377, DateTimeKind.Utc).AddTicks(6160),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 2, 22, 11, 8, 33, 489, DateTimeKind.Utc).AddTicks(5119));

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
                defaultValue: new DateTime(2026, 5, 25, 13, 47, 54, 377, DateTimeKind.Utc).AddTicks(4831),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValue: new DateTime(2026, 2, 22, 11, 8, 33, 489, DateTimeKind.Utc).AddTicks(3836));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 25, 13, 47, 54, 377, DateTimeKind.Utc).AddTicks(4575),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 2, 22, 11, 8, 33, 489, DateTimeKind.Utc).AddTicks(3569));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "DiscountUsages");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UsedAt",
                table: "DiscountUsages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 22, 11, 8, 33, 489, DateTimeKind.Utc).AddTicks(5119),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 5, 25, 13, 47, 54, 377, DateTimeKind.Utc).AddTicks(6160));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: true,
                defaultValue: new DateTime(2026, 2, 22, 11, 8, 33, 489, DateTimeKind.Utc).AddTicks(3836),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValue: new DateTime(2026, 5, 25, 13, 47, 54, 377, DateTimeKind.Utc).AddTicks(4831));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 22, 11, 8, 33, 489, DateTimeKind.Utc).AddTicks(3569),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 5, 25, 13, 47, 54, 377, DateTimeKind.Utc).AddTicks(4575));
        }
    }
}
