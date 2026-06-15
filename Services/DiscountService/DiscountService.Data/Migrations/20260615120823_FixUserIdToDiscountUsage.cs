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
                defaultValue: new DateTime(2026, 6, 15, 12, 8, 22, 743, DateTimeKind.Utc).AddTicks(5781),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 6, 14, 9, 5, 13, 989, DateTimeKind.Utc).AddTicks(6293));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: true,
                defaultValue: new DateTime(2026, 6, 15, 12, 8, 22, 743, DateTimeKind.Utc).AddTicks(4309),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValue: new DateTime(2026, 6, 14, 9, 5, 13, 989, DateTimeKind.Utc).AddTicks(4987));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 15, 12, 8, 22, 743, DateTimeKind.Utc).AddTicks(4044),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 6, 14, 9, 5, 13, 989, DateTimeKind.Utc).AddTicks(4728));
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
                defaultValue: new DateTime(2026, 6, 14, 9, 5, 13, 989, DateTimeKind.Utc).AddTicks(6293),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 6, 15, 12, 8, 22, 743, DateTimeKind.Utc).AddTicks(5781));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: true,
                defaultValue: new DateTime(2026, 6, 14, 9, 5, 13, 989, DateTimeKind.Utc).AddTicks(4987),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValue: new DateTime(2026, 6, 15, 12, 8, 22, 743, DateTimeKind.Utc).AddTicks(4309));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 6, 14, 9, 5, 13, 989, DateTimeKind.Utc).AddTicks(4728),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 6, 15, 12, 8, 22, 743, DateTimeKind.Utc).AddTicks(4044));
        }
    }
}
