using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_CEP.Migrations
{
    /// <inheritdoc />
    public partial class CreateCepEnderecoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cep_endereco",
                columns: table => new
                {
                    cep = table.Column<string>(type: "char(8)", nullable: false),
                    logradouro = table.Column<string>(type: "text", nullable: false),
                    complemento = table.Column<string>(type: "text", nullable: false),
                    bairro = table.Column<string>(type: "text", nullable: false),
                    localidade = table.Column<string>(type: "text", nullable: false),
                    uf = table.Column<string>(type: "text", nullable: false),
                    ibge = table.Column<string>(type: "text", nullable: false),
                    gia = table.Column<string>(type: "text", nullable: false),
                    ddd = table.Column<string>(type: "text", nullable: false),
                    siafi = table.Column<string>(type: "text", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cep_endereco", x => x.cep);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cep_endereco");
        }
    }
}
