using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.model.adm
{
    [Table("permissao")]
    public class Permissao
    {
        [Column("id_conta")]
        public long IdConta { get; set; }
        [Column("id_permissao")]
        public long IdPermissao { get; set; }
    }
}
