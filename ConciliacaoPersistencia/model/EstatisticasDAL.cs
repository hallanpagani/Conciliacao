using System;
using System.Data;
using System.Text;
using ConciliacaoPersistencia.banco;

namespace ConciliacaoPersistencia.model
{
    public class EstatisticasDAL
    {
        #region estoque
        // posicao do estoque (valores)
        public static DataTable GetPosicaoEstoque()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select vl_tot_custo, vl_tot_venda, ((vl_tot_venda - vl_tot_custo) / vl_tot_custo) * 100 as margem_media ");
            sb.Append("from ");
            sb.Append("( ");
            sb.Append("select sum(case when qtd > 0 then vl_venda * qtd else 0 end) as vl_tot_venda, ");
            sb.Append("       sum(case when qtd > 0 then vl_custo * qtd else 0 end) as vl_tot_custo ");
            sb.Append("from tb_produto ");
            sb.Append(") as x ");
            return DAL.ListarFromSQL(sb.ToString());
        }

        // top 10 produtos com mais estoque
        public static DataTable GetTop10ProdutoComMaisEstoque()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select id_produto, nome, qtd ");
            sb.Append("from tb_produto ");
            sb.Append("order by qtd desc ");
            sb.Append("limit 10 ");
            return DAL.ListarFromSQL(sb.ToString());
        }

        // top 10 produto menos lucrativos
        public static DataTable GetTop10MenosLucrativos()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select id_produto, nome, qtd, vl_venda, vl_custo, ((vl_venda - vl_custo) / vl_custo) * 100 as margem ");
            sb.Append("from tb_produto ");
            sb.Append("order by margem  ");
            sb.Append("limit 10 ");
            return DAL.ListarFromSQL(sb.ToString());
        }

        // top 10 produtos com menos estoque
        public static DataTable GetTop10ComMenosEstoque()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select id_produto, nome, qtd ");
            sb.Append("from tb_produto ");
            sb.Append("order by qtd  ");
            sb.Append("limit 10 ");
            return DAL.ListarFromSQL(sb.ToString());
        }

        // top 10 produto mais lucrativos
        public static DataTable GetTop10Lucrativos()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select id_produto, nome, qtd, vl_venda, vl_custo, ((vl_venda - vl_custo) / vl_custo) * 100 as margem ");
            sb.Append("from tb_produto ");
            sb.Append("order by margem desc ");
            sb.Append("limit 10 ");
            return DAL.ListarFromSQL(sb.ToString());
        }
        #endregion

        #region venda

        protected static string GetPeriodo(DateTime dataIni, DateTime dataFim)
        {
            return string.Format(" v.data between '{0}' and '{1}' ", dataIni.ToString("yyyy-MM-dd"), dataFim.ToString("yyyy-MM-dd"));
        }

        // top 10 produtos mais vendidos em quantidade
        public static DataTable GetTop10ProdutosMaisVendidosPorQtd(DateTime dataIni, DateTime dataFim)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select v.id_produto, nome, sum(v.qtd) as quant, sum(v.vl_venda * v.qtd) as valor ");
            sb.Append("from tb_venda_produto v ");
            sb.Append("join tb_produto p on p.id_produto = v.id_produto ");
            sb.Append("where ");
            sb.Append(GetPeriodo(dataIni, dataFim));
            sb.Append("group by v.id_produto, nome ");
            sb.Append("order by 3 desc, 4 desc ");
            sb.Append("limit 10 ");
            return DAL.ListarFromSQL(sb.ToString());
        }

        // top 10 produtos mais vendidos em valor
        public static DataTable GetTop10ProdutosMaisVendidosPorValor(DateTime dataIni, DateTime dataFim)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select v.id_produto, nome, sum(v.qtd) as quant, sum(v.vl_venda * v.qtd) as valor ");
            sb.Append("from tb_venda_produto v ");
            sb.Append("join tb_produto p on p.id_produto = v.id_produto ");
            sb.Append("where ");
            sb.Append(GetPeriodo(dataIni, dataFim));
            sb.Append("group by v.id_produto, nome ");
            sb.Append("order by 4 desc, 3 desc ");
            sb.Append("limit 10 ");
            return DAL.ListarFromSQL(sb.ToString());
        }

        // top 10 produtos mais lucrativos
        public static DataTable GetTop10ProdutosMaisVendidosPorLucratividade(DateTime dataIni, DateTime dataFim)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select v.id_produto, nome, sum(v.qtd) as quant,  ");
            sb.Append("       sum(v.vl_custo * v.qtd) as custo, sum(v.vl_venda * v.qtd) as valor, ");
            sb.Append("       avg(((v.vl_venda - v.vl_custo) / v.vl_custo) * 100) as margem_media ");
            sb.Append("from tb_venda_produto v ");
            sb.Append("join tb_produto p on p.id_produto = v.id_produto ");
            sb.Append("where ");
            sb.Append(GetPeriodo(dataIni, dataFim));
            sb.Append("group by v.id_produto, nome ");
            sb.Append("order by 6 desc ");
            sb.Append("limit 10 ");
            return DAL.ListarFromSQL(sb.ToString());
        }
        #endregion
    }
}