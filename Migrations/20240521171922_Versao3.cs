using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoFinal_Myte_Grupo3.Migrations
{
    /// <inheritdoc />
    public partial class Versao3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "WBS",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "SequentialCounter",
                table: "WBS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "WBS",
                keyColumn: "WBSId",
                keyValue: 1,
                column: "SequentialCounter",
                value: 0);

            migrationBuilder.UpdateData(
                table: "WBS",
                keyColumn: "WBSId",
                keyValue: 2,
                column: "SequentialCounter",
                value: 0);

            migrationBuilder.UpdateData(
                table: "WBS",
                keyColumn: "WBSId",
                keyValue: 3,
                column: "SequentialCounter",
                value: 0);

            migrationBuilder.UpdateData(
                table: "WBS",
                keyColumn: "WBSId",
                keyValue: 4,
                column: "SequentialCounter",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SequentialCounter",
                table: "WBS");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "WBS",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);
        }
    }
}
