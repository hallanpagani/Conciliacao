using System.Data;
using System.Text;
using ConciliacaoPersistencia.banco;

namespace ConciliacaoPersistencia.Model
{
    public static class ClienteDAL
    {
        public static DataTable GetAniversariantes()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select p.nome, DAY(nasc) as dia, m.nome as mes ");
            sb.Append("from tb_cliente p ");
            sb.Append("join tb_mes m on m.id_mes = month(nasc) ");
            sb.Append("where month(nasc) >= month(current_date) and month(nasc) <= month(current_date)+1 ");
            sb.Append("order by month(nasc), day(nasc) ");
            return DAL.ListarFromSQL(sb.ToString());
        }
    }
}