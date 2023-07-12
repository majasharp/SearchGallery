using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SearchGallery.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddContentTypeToGalleryItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "GalleryItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "GalleryItems");
        }
    }
}
