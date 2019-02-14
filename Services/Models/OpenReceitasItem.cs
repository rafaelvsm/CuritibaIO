using System;

namespace Services.Models
{
    public class OpenReceitasItem
    {
        public string _id { get; set; }
        public string CD_RECEITA { get; set; }
        public string CD_CATEGORIA { get; set; }
        public string DESCRICAO_CATEGORIA { get; set; }
        public string CD_ORIGEM { get; set; }
        public string DESCRICAO_ORIGEM { get; set; }
        public string CD_ESPECIE { get; set; }
        public string DESCRICAO_ESPECIE { get; set; }
        public string CD_RUBRICA { get; set; }
        public string DESCRICAO_RUBRICA { get; set; }
        public string CD_ALINEA { get; set; }
        public string DESCRICAO_ALINEA { get; set; }
        public string CD_SUBALINEA { get; set; }
        public string DESCRICAO_SUBALINEA { get; set; }
        public string CD_EXERCICIO { get; set; }
        public DateTime DT_APROPRIACAO { get; set; }
        public string TP_RECEITA_ORC { get; set; }
        public string CD_EMPRESA { get; set; }
        public string NM_EMPRESA { get; set; }
        public decimal VL_RECEITA { get; set; }
        public string DS_FONTE { get; set; }
    }
}
