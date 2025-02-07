using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace taskmanager.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectTasksNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ProjectTasks_TaskID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_AssignedUserID",
                table: "ProjectTasks");

            migrationBuilder.AlterColumn<int>(
                name: "TaskID",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ProjectTasks_TaskID",
                table: "Notifications",
                column: "TaskID",
                principalTable: "ProjectTasks",
                principalColumn: "ProjectTaskID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_AssignedUserID",
                table: "ProjectTasks",
                column: "AssignedUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ProjectTasks_TaskID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_AssignedUserID",
                table: "ProjectTasks");

            migrationBuilder.AlterColumn<int>(
                name: "TaskID",
                table: "Notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ProjectTasks_TaskID",
                table: "Notifications",
                column: "TaskID",
                principalTable: "ProjectTasks",
                principalColumn: "ProjectTaskID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_AssignedUserID",
                table: "ProjectTasks",
                column: "AssignedUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
