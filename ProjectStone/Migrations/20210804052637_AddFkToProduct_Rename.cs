using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectStone.Migrations
{
    public partial class AddFkToProduct_Rename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_SubCategory_SubCategoryId",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "SubCategoryId",
                table: "Product",
                newName: "SubCategoryTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_SubCategoryId",
                table: "Product",
                newName: "IX_Product_SubCategoryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_SubCategory_SubCategoryTypeId",
                table: "Product",
                column: "SubCategoryTypeId",
                principalTable: "SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_SubCategory_SubCategoryTypeId",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "SubCategoryTypeId",
                table: "Product",
                newName: "SubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_SubCategoryTypeId",
                table: "Product",
                newName: "IX_Product_SubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_SubCategory_SubCategoryId",
                table: "Product",
                column: "SubCategoryId",
                principalTable: "SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
