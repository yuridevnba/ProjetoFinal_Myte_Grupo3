using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoFinal_Myte_Grupo3.Data.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigrat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusEmployee",
                table: "Employee",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusEmployee",
                table: "Employee");
        }
    }
}
