using System;
namespace Services.Models
{
    public class OpenDespesasItem
    {
        public string _id { get; set; }
        public int ANO_EMPENHO { get; set; }
        public DateTime DT_EMPENHO { get; set; }
        public string CD_FONTE { get; set; }
        public string DS_FONTE { get; set; }
        public string CD_FUNCAO { get; set; }
        public string DS_FUNCAO { get; set; }
        public string CD_PROGRAMA { get; set; }
        public string DS_PROGRAMA { get; set; }
        public string CD_ACAO { get; set; }
        public string DS_ACAO { get; set; }
        public string CD_SUBELEMENTO { get; set; }
        public string DS_SUBELEMENTO { get; set; }
        public string CD_ORGAO { get; set; }
        public string DS_ORGAO { get; set; }
        public string CD_DESPESA { get; set; }
        public string DS_DESPESA { get; set; }
        public string CODIGO_DESPESA_GRUPO { get; set; }
        public string DS_GRUPO { get; set; }
        public string CODIGO_DESPESA_MODALIDADE { get; set; }
        public string DS_MODALIDADE { get; set; }
        public string CODIGO_DESPESA_ELEMENTO { get; set; }
        public string DS_ELEMENTO { get; set; }
        public string CPF_CNPJ { get; set; }
        public int NR_EMPENHO { get; set; }
        public string LICITACAO { get; set; }
        public decimal VL_EMPENHADO { get; set; }
        public string CD_ITEM { get; set; }
        public string DS_ITEM { get; set; }
        public string DS_UNIDADE { get; set; }
        public int? QUANTIDADE { get; set; }
        public decimal? VL_PRECO_UNITARIO { get; set; }
        public decimal? VL_TOTAL { get; set; }
        public string PROTOCOLOSUP { get; set; }
        public DateTime? DT_TRANSACAO { get; set; }
        public int? NR_PARCELA { get; set; }
        public string TRANSACAO { get; set; }        
        public decimal? VL_LIQUIDADO { get; set; }
        public decimal? VL_DEVOLVIDO { get; set; }
        public decimal? VL_ANULADO { get; set; }
        public decimal? VL_PAGO { get; set; }
        public decimal? VL_CONSIGNADO { get; set; }
    }
}
