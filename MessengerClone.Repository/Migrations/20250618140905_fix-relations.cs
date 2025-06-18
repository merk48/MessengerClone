using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessengerClone.Repository.Migrations
{
    /// <inheritdoc />
    public partial class fixrelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatMembers_SenderId_ChatId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatMembers_SenderId_ChatId",
                table: "Messages",
                columns: new[] { "SenderId", "ChatId" },
                principalTable: "ChatMembers",
                principalColumns: new[] { "UserId", "ChatId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatMembers_SenderId_ChatId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatMembers_SenderId_ChatId",
                table: "Messages",
                columns: new[] { "SenderId", "ChatId" },
                principalTable: "ChatMembers",
                principalColumns: new[] { "UserId", "ChatId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
