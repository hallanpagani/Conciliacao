using ConciliacaoModelo.model.generico;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConciliacaoModelo.model.conciliador
{
    [Table("conciliador_arquivos_md5")]
    public class ArquivoMD5 : BaseID
    {
        [Column("arquivo_md5")]
        public string arquivo_md5 { get; set; }

        public ArquivoMD5()
        {
            arquivo_md5 = "";
        }

    }
}
