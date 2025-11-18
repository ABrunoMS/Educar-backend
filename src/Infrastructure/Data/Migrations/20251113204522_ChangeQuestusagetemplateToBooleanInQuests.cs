using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeQuestusagetemplateToBooleanInQuests : Migration
    {
 
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
                ALTER TABLE ""quests""
        ALTER COLUMN ""usage_template"" TYPE boolean
        USING (
            CASE
                WHEN ""usage_template"" = 'Global' THEN true
                ELSE false
            END
        )::boolean;
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""quests"" 
                ALTER COLUMN ""usage_template"" TYPE text 
                USING usage_template::text;
            ");
        }
    }
}