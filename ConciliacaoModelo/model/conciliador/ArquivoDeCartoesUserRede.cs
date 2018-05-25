using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using ConciliacaoModelo.model.conciliador.UseRede;

namespace ConciliacaoModelo.model.conciliador
{
    public class ArquivoDeCartoesUserRede
    {
        [Column("dt_arquivo")]
        [Display(Name = "Data do arquivo")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date, ErrorMessage = "Data em formato inválido")]
        public string dataarquivo { get; set; }

        [Column("dt_inicial")]
        [Display(Name = "Data Inicial")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date, ErrorMessage = "Data em formato inválido")]
        public string datainicial { get; set; }

        [Column("dt_final")]
        [Display(Name = "Data final")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date, ErrorMessage = "Data em formato inválido")]
        public string datafinal { get; set; }

        [Column("tp_arquivo")]
        [Display(Name = "Descrição")]
        public string tp_arquivo { get; set; }

        [Column("ds_arquivo")]
        [Display(Name = "Arquivo processado")]
        public string ds_arquivo { get; set; }

        public HttpPostedFileBase arquivo { get; set; }

        public List<ConciliacaoUseRedeEEVDResumoOperacaoStruct> ArquivosDeCartoesEEVDUserRede { get; set; }
        public List<ConciliacaoUseRedeEEVCResumoOperacaoStruct> ArquivosDeCartoesEEVCUserRede { get; set; }
        public List<ConciliacaoUseRedeEEFIResumoOperacaoStruct> ArquivosDeCartoesEEFIUserRede { get; set; }



    }
}
