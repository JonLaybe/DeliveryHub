using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscountService.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderIdAndUserIdToDiscountUsage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "DiscountUsages",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UsedAt",
                table: "DiscountUsages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 25, 13, 51, 29, 995, DateTimeKind.Utc).AddTicks(1754),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 5, 25, 13, 47, 54, 377, DateTimeKind.Utc).AddTicks(6160));

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "DiscountUsages",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: true,
                defaultValue: new DateTime(2026, 5, 25, 13, 51, 29, 995, DateTimeKind.Utc).AddTicks(529),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValue: new DateTime(2026, 5, 25, 13, 47, 54, 377, DateTimeKind.Utc).AddTicks(4831));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 25, 13, 51, 29, 995, DateTimeKind.Utc).AddTicks(297),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 5, 25, 13, 47, 54, 377, DateTimeKind.Utc).AddTicks(4575));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "DiscountUsages",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UsedAt",
                table: "DiscountUsages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 25, 13, 47, 54, 377, DateTimeKind.Utc).AddTicks(6160),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 5, 25, 13, 51, 29, 995, DateTimeKind.Utc).AddTicks(1754));

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "DiscountUsages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: true,
                defaultValue: new DateTime(2026, 5, 25, 13, 47, 54, 377, DateTimeKind.Utc).AddTicks(4831),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValue: new DateTime(2026, 5, 25, 13, 51, 29, 995, DateTimeKind.Utc).AddTicks(529));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 5, 25, 13, 47, 54, 377, DateTimeKind.Utc).AddTicks(4575),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2026, 5, 25, 13, 51, 29, 995, DateTimeKind.Utc).AddTicks(297));
        }
    }
}
