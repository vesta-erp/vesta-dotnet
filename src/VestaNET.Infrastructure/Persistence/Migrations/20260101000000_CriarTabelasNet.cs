using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VestaNET.Infrastructure.Persistence.Migrations;

public partial class CriarTabelasNet : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TB_NET_ANALISE",
            columns: table => new
            {
                ID_ANALISE       = table.Column<long>(type: "NUMBER(19)", nullable: false)
                    .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                ID_ABRIGO        = table.Column<long>(type: "NUMBER(19)", nullable: false),
                VL_SCORE         = table.Column<double>(type: "NUMBER(5,2)", nullable: false),
                TP_NIVEL         = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                DS_JUSTIFICATIVA = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                DT_ANALISE       = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TB_NET_ANALISE", x => x.ID_ANALISE);
            });

        migrationBuilder.CreateTable(
            name: "TB_NET_RECOMENDACAO",
            columns: table => new
            {
                ID_RECOMENDACAO = table.Column<long>(type: "NUMBER(19)", nullable: false)
                    .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                ID_ANALISE      = table.Column<long>(type: "NUMBER(19)", nullable: false),
                TP_RECOMENDACAO = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                DS_DESCRICAO    = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TB_NET_RECOMENDACAO", x => x.ID_RECOMENDACAO);
                table.ForeignKey(
                    name: "FK_NET_REC_ANALISE",
                    column: x => x.ID_ANALISE,
                    principalTable: "TB_NET_ANALISE",
                    principalColumn: "ID_ANALISE",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex("IX_NET_ANALISE_ABRIGO", "TB_NET_ANALISE",      "ID_ABRIGO");
        migrationBuilder.CreateIndex("IX_NET_REC_ANALISE",    "TB_NET_RECOMENDACAO", "ID_ANALISE");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("TB_NET_RECOMENDACAO");
        migrationBuilder.DropTable("TB_NET_ANALISE");
    }
}
