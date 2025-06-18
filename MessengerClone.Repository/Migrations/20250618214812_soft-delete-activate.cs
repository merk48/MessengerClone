using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessengerClone.Repository.Migrations
{
    /// <inheritdoc />
    public partial class softdeleteactivate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeleted",
                table: "MessageStatuses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "MessageStatuses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MessageStatuses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeleted",
                table: "MessageReactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "MessageReactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MessageReactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeleted",
                table: "ChatMembers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "ChatMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ChatMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_MessageStatuses_DeletedBy",
                table: "MessageStatuses",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactions_DeletedBy",
                table: "MessageReactions",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMembers_DeletedBy",
                table: "ChatMembers",
                column: "DeletedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMembers_AspNetUsers_DeletedBy",
                table: "ChatMembers",
                column: "DeletedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReactions_AspNetUsers_DeletedBy",
                table: "MessageReactions",
                column: "DeletedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageStatuses_AspNetUsers_DeletedBy",
                table: "MessageStatuses",
                column: "DeletedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMembers_AspNetUsers_DeletedBy",
                table: "ChatMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReactions_AspNetUsers_DeletedBy",
                table: "MessageReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageStatuses_AspNetUsers_DeletedBy",
                table: "MessageStatuses");

            migrationBuilder.DropIndex(
                name: "IX_MessageStatuses_DeletedBy",
                table: "MessageStatuses");

            migrationBuilder.DropIndex(
                name: "IX_MessageReactions_DeletedBy",
                table: "MessageReactions");

            migrationBuilder.DropIndex(
                name: "IX_ChatMembers_DeletedBy",
                table: "ChatMembers");

            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "MessageStatuses");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "MessageStatuses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MessageStatuses");

            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "MessageReactions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "MessageReactions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MessageReactions");

            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "ChatMembers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ChatMembers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ChatMembers");
        }
    }
}
