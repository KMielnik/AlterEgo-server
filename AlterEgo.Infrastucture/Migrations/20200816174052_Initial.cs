using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AlterEgo.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Login = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nickname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mail = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Login);
                });

            migrationBuilder.CreateTable(
                name: "DrivingVideos",
                columns: table => new
                {
                    Filename = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OriginalFilename = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OwnerLogin = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedDeletion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualDeletion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Thumbnail = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PlannedLifetime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrivingVideos", x => x.Filename);
                    table.ForeignKey(
                        name: "FK_DrivingVideos_Users_OwnerLogin",
                        column: x => x.OwnerLogin,
                        principalTable: "Users",
                        principalColumn: "Login",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Filename = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OriginalFilename = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OwnerLogin = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedDeletion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualDeletion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Thumbnail = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PlannedLifetime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Filename);
                    table.ForeignKey(
                        name: "FK_Images_Users_OwnerLogin",
                        column: x => x.OwnerLogin,
                        principalTable: "Users",
                        principalColumn: "Login",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResultVideos",
                columns: table => new
                {
                    Filename = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OriginalFilename = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OwnerLogin = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedDeletion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualDeletion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Thumbnail = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PlannedLifetime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsFinished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultVideos", x => x.Filename);
                    table.ForeignKey(
                        name: "FK_ResultVideos_Users_OwnerLogin",
                        column: x => x.OwnerLogin,
                        principalTable: "Users",
                        principalColumn: "Login",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnimationTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerLogin = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SourceVideoFilename = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SourceImageFilename = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ResultAnimationFilename = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RetainAudio = table.Column<bool>(type: "bit", nullable: false),
                    ImagePadding = table.Column<float>(type: "real", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimationTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimationTasks_DrivingVideos_SourceVideoFilename",
                        column: x => x.SourceVideoFilename,
                        principalTable: "DrivingVideos",
                        principalColumn: "Filename",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnimationTasks_Images_SourceImageFilename",
                        column: x => x.SourceImageFilename,
                        principalTable: "Images",
                        principalColumn: "Filename",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnimationTasks_ResultVideos_ResultAnimationFilename",
                        column: x => x.ResultAnimationFilename,
                        principalTable: "ResultVideos",
                        principalColumn: "Filename",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnimationTasks_Users_OwnerLogin",
                        column: x => x.OwnerLogin,
                        principalTable: "Users",
                        principalColumn: "Login",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnimationTasks_OwnerLogin",
                table: "AnimationTasks",
                column: "OwnerLogin");

            migrationBuilder.CreateIndex(
                name: "IX_AnimationTasks_ResultAnimationFilename",
                table: "AnimationTasks",
                column: "ResultAnimationFilename");

            migrationBuilder.CreateIndex(
                name: "IX_AnimationTasks_SourceImageFilename",
                table: "AnimationTasks",
                column: "SourceImageFilename");

            migrationBuilder.CreateIndex(
                name: "IX_AnimationTasks_SourceVideoFilename",
                table: "AnimationTasks",
                column: "SourceVideoFilename");

            migrationBuilder.CreateIndex(
                name: "IX_DrivingVideos_OwnerLogin",
                table: "DrivingVideos",
                column: "OwnerLogin");

            migrationBuilder.CreateIndex(
                name: "IX_Images_OwnerLogin",
                table: "Images",
                column: "OwnerLogin");

            migrationBuilder.CreateIndex(
                name: "IX_ResultVideos_OwnerLogin",
                table: "ResultVideos",
                column: "OwnerLogin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimationTasks");

            migrationBuilder.DropTable(
                name: "DrivingVideos");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "ResultVideos");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
