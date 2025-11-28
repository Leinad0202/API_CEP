using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_CEP.Models
{
    [Table("cep_endereco")]
    public class CepEndereco
    {
        [Key]
        [Column("cep", TypeName = "char(8)")]
        public string Cep { get; set; }

        [Column("logradouro")]
        public string Logradouro { get; set; }

        [Column("complemento")]
        public string Complemento { get; set; }

        [Column("bairro")]
        public string Bairro { get; set; }

        [Column("localidade")]
        public string Localidade { get; set; }

        [Column("uf")]
        public string Uf { get; set; }

        [Column("ibge")]
        public string Ibge { get; set; }

        [Column("gia")]
        public string Gia { get; set; }

        [Column("ddd")]
        public string Ddd { get; set; }

        [Column("siafi")]
        public string Siafi { get; set; }

        [Column("atualizado_em")]
        public DateTime? AtualizadoEm { get; set; } // único nullable necessário
    }
}
