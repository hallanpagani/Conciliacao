using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Conciliacao.Models.conciliacao
{
    public class ConciliacaoHeaderStruct : Controller
    {

        /*
        Variaveis de trabalho.
        */
        public string is_tipo_registro;
        public string is_estabelecimento_matriz;
        public string is_data_processamento;
        public string is_periodo_inicial;
        public string is_periodo_final;
        public string is_sequencia;
        public string is_empresa_adquirente;
        public string is_opcao_extrato;
        public string is_van;
        public string is_caixa_postal;
        public string is_versao_layout;

        /*
        Construtor da classe.
        */
        public ConciliacaoHeaderStruct()
        {

        }

        // GET: ConciliacaoHeaderStruct
        public ActionResult Index()
        {
            return View();
        }
    }
}