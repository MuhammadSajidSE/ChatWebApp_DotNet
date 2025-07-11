using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatWebApp.Migrations
{
    /// <inheritdoc />
    public partial class Crea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContactId",
                table: "Contacts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Contacts");
        }
    }
}
