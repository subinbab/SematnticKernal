using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SemanticAIApp.Migrations
{
    /// <inheritdoc />
    public partial class _2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpenAISubscription_AspNetUsers_UserId1",
                table: "OpenAISubscription");

            migrationBuilder.DropIndex(
                name: "IX_OpenAISubscription_UserId1",
                table: "OpenAISubscription");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "07b18e46-6856-40b1-a50b-c1d2d964727c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c0f53d98-59e1-4f05-8037-73d999f958c1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c288f29b-f9f2-47cd-8139-7e474f86aecc");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "OpenAISubscription");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "OpenAISubscription",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1ae412f6-e5c3-4b26-bae8-5c69e8ff3448", "2", "ROLE1", "ROLE1" },
                    { "5ec2d3c6-77af-459d-8aad-39b8be06f9d7", "1", "ADMIN", "ADMIN" },
                    { "844f1ef3-562a-4a6a-a95e-2e7c94609b5e", "3", "ROLE2", "ROLE2" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpenAISubscription_UserId",
                table: "OpenAISubscription",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OpenAISubscription_AspNetUsers_UserId",
                table: "OpenAISubscription",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpenAISubscription_AspNetUsers_UserId",
                table: "OpenAISubscription");

            migrationBuilder.DropIndex(
                name: "IX_OpenAISubscription_UserId",
                table: "OpenAISubscription");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1ae412f6-e5c3-4b26-bae8-5c69e8ff3448");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ec2d3c6-77af-459d-8aad-39b8be06f9d7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "844f1ef3-562a-4a6a-a95e-2e7c94609b5e");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "OpenAISubscription",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "OpenAISubscription",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "07b18e46-6856-40b1-a50b-c1d2d964727c", "2", "ROLE1", "ROLE1" },
                    { "c0f53d98-59e1-4f05-8037-73d999f958c1", "3", "ROLE2", "ROLE2" },
                    { "c288f29b-f9f2-47cd-8139-7e474f86aecc", "1", "ADMIN", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpenAISubscription_UserId1",
                table: "OpenAISubscription",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OpenAISubscription_AspNetUsers_UserId1",
                table: "OpenAISubscription",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
