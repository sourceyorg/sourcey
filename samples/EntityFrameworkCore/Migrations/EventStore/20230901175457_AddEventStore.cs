using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFrameworkCore.Migrations.EventStore
{
    /// <inheritdoc />
    public partial class AddEventStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "log");

            migrationBuilder.CreateTable(
                name: "Event",
                schema: "log",
                columns: table => new
                {
                    SequenceNo = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StreamId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Correlation = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Causation = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Actor = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ScheduledPublication = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.SequenceNo);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_Actor",
                schema: "log",
                table: "Event",
                column: "Actor");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Causation",
                schema: "log",
                table: "Event",
                column: "Causation");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Correlation",
                schema: "log",
                table: "Event",
                column: "Correlation");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Id",
                schema: "log",
                table: "Event",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Name",
                schema: "log",
                table: "Event",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Event_StreamId",
                schema: "log",
                table: "Event",
                column: "StreamId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Version",
                schema: "log",
                table: "Event",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Event",
                schema: "log");
        }
    }
}
