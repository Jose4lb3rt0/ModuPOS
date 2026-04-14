using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModuPOS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddImagenToProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImagenId",
                table: "Productos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ImagenId",
                table: "Productos",
                column: "ImagenId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Imagenes_ImagenId",
                table: "Productos",
                column: "ImagenId",
                principalTable: "Imagenes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Imagenes_ImagenId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_ImagenId",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ImagenId",
                table: "Productos");
        }
    }
}
