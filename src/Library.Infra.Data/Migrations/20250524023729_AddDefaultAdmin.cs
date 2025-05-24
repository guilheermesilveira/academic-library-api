using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Infra.Data.Migrations
{
    public partial class AddDefaultAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var password = "$argon2id$v=19$m=32768,t=4,p=1$8kSN61J8u9f2fBanH2sbjA$mcjis6H1GOwjNVVNBznVkOkktsa+CHUc9bP95x8IsEo";
            
            migrationBuilder.InsertData(
                table: "Administrators",
                columns: new[] { "Id", "Name", "Email", "Password" },
                values: new object[,]
                {
                    { 1, "Administrator", "admin@admin.com", password }
                }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Administrators",
                keyColumn: "Id",
                keyValue: 1
            );
        }
    }
}
