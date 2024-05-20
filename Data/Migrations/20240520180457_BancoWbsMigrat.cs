using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjetoFinal_Myte_Grupo3.Data.Migrations
{
    /// <inheritdoc />
    public partial class BancoWbsMigrat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "WBS",
                columns: new[] { "WBSId", "Code", "Description", "Type" },
                values: new object[,]
                {
                    { 1, "WBS0000001", "Férias", "Non-Chargeability" },
                    { 2, "WBS0000002", "Day-Off", "Non-Chargeability" },
                    { 3, "WBS0000003", "Sem Tarefa", "Non-Chargeability" },
                    { 4, "WBS0000004", "Implementação e Desenvolvimento", "Chargeability" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WBS",
                keyColumn: "WBSId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "WBS",
                keyColumn: "WBSId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "WBS",
                keyColumn: "WBSId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "WBS",
                keyColumn: "WBSId",
                keyValue: 4);
        }
    }
}
