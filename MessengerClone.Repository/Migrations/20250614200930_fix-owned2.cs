using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessengerClone.Repository.Migrations
{
    /// <inheritdoc />
    public partial class fixowned2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastMessageType",
                table: "Chats",
                newName: "LastMessage_Type");

            migrationBuilder.RenameColumn(
                name: "LastMessageSentAt",
                table: "Chats",
                newName: "LastMessage_SentAt");

            migrationBuilder.RenameColumn(
                name: "LastMessageSenderUsername",
                table: "Chats",
                newName: "LastMessage_SenderUsername");

            migrationBuilder.RenameColumn(
                name: "LastMessageId",
                table: "Chats",
                newName: "LastMessage_Id");

            migrationBuilder.RenameColumn(
                name: "LastMessageContent",
                table: "Chats",
                newName: "LastMessage_Content");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastMessage_Type",
                table: "Chats",
                newName: "LastMessageType");

            migrationBuilder.RenameColumn(
                name: "LastMessage_SentAt",
                table: "Chats",
                newName: "LastMessageSentAt");

            migrationBuilder.RenameColumn(
                name: "LastMessage_SenderUsername",
                table: "Chats",
                newName: "LastMessageSenderUsername");

            migrationBuilder.RenameColumn(
                name: "LastMessage_Id",
                table: "Chats",
                newName: "LastMessageId");

            migrationBuilder.RenameColumn(
                name: "LastMessage_Content",
                table: "Chats",
                newName: "LastMessageContent");
        }
    }
}
