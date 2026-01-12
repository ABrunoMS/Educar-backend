using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Educar.Backend.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSequenceToQuestStepContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sequence",
                table: "quest_step_contents",
                type: "integer",
                nullable: false,
                defaultValue: 1);
            
            // Atualizar registros existentes com sequência baseada na ordem de criação
            migrationBuilder.Sql(@"
                WITH numbered_contents AS (
                    SELECT id, 
                           ROW_NUMBER() OVER (PARTITION BY quest_step_id ORDER BY created) as row_num
                    FROM quest_step_contents
                )
                UPDATE quest_step_contents qsc
                SET sequence = nc.row_num
                FROM numbered_contents nc
                WHERE qsc.id = nc.id;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sequence",
                table: "quest_step_contents");
        }
    }
}
