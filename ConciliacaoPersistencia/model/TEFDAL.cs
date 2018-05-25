using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConciliacaoModelo.model.cadastros;
using ConciliacaoModelo.model.conciliador;
using ConciliacaoPersistencia.banco;
using Funcoes.classes;
using Conciliacao.Models;
using Oracle.ManagedDataAccess.Client;
using ConciliacaoPersistencia.classes;

namespace ConciliacaoPersistencia.model
{
    public class TEFDAL
    {
        public static RetornoTEF GetStatusTEF(long idconta, string nsu_rede)
        {
            if (String.IsNullOrEmpty(nsu_rede))
            {
                return new RetornoTEF();
            }

            var rtef = new RetornoTEF();

          //  var tef = DAL.GetObjeto<BancoTEF>(string.Format("id_conta ={0}", idconta));

          /*  var nome_cliente =
                DAL.GetString(string.Format("select nm_tef from sistema_conta where id={0}", idconta), "erro"); */

            //  "Data Source=jdbc:oracle:thin:@172.31.255.20:1521:xe;User Id=vspague_Matheusnext;Password=administrador123#"

            // server
          /*  string str_con =
                string.Format(
                    @"User Id={0};Password=""{1}"";Data Source=(DESCRIPTION =" +
                    "(ADDRESS = (PROTOCOL = TCP)(HOST = {2})(PORT = {3}))" + "(CONNECT_DATA =" +
                    "(SERVER = DEDICATED)" + "(SERVICE_NAME = XE)))", tef.usuario_banco, tef.senha_banco,
                    tef.ip_banco, tef.porta_banco); */

           // using (ConexaoOracle conexao = ConexaoOracle.Get(str_con))

            //  local
            // using (ConexaoOracle conexao = ConexaoOracle.Get(@"User Id=vspague_suporte;Password=""@s0t3cht1"";Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)(HOST = 172.31.255.20)(PORT = 1521))" + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = XE)))"))
            {
                /* using (Conexao conexao2 = Conexao.Get(DAL.GetStringConexao()))
                using (Transacao transacao = new Transacao(conexao2))
                    try
                    {
                        string sql2 = "insert into sistema_erro(mensagem) value ('conectado'); ";
                        using (Comando comando = new Comando(transacao, sql2))
                        {
                            comando.Execute();
                        }
                        transacao.Commit();
                    }
                    catch (Exception ex)
                    {
                        transacao.RollBack();
                        throw ex;
                    } */


                /*

                string sql = String.Format(@"select Sequencial, Transacao_Nsu,  Transacao_Nsu_Rede,

      Transacao_Autorizacao, 
      Transacao_Identificacao,
      Administrador,
      Cartao_numero


from VALOR_LOJA_TRANSACAO_V775 where TRANSACAO_NSU_REDE = '{0}'", nsu_rede);  */




                Leitor l = DAL.GetLeitorFromSQL(@"SELECT Sequencial, Transacao_Nsu,Transacao_Nsu_Rede,Transacao_Autorizacao,Transacao_Identificacao, Administrador,Cartao_numero, id_conta FROM cadastro_transacoes_tef where " + String.Format(@"id_conta = {0} and ((cast(Transacao_Nsu_Rede as UNSIGNED ) = {1}) or (cast(Transacao_Nsu as UNSIGNED ) = {1})) ", idconta, nsu_rede));

                // using (ComandoOracle comando = new ComandoOracle(conexao, sql))
                //  {
                //     var retorno = comando.Select(sql);

                while (!l.Eof)
                {

                    rtef.CodigoTEF = l.GetString("Sequencial");
                    rtef.NSUTEF = l.GetString("Transacao_Nsu");
                    rtef.NSUREDE = l.GetString("Transacao_Nsu_Rede");
                    rtef.Autorizacao = l.GetString("Transacao_Autorizacao");
                    rtef.Identificacao = l.GetString("Transacao_Identificacao");
                    rtef.Bandeira = l.GetString("Administrador");
                    rtef.Cartao = l.GetString("Cartao_numero");
                    l.Next();

                }
                // Log.Sql(sql);
                // }

                return rtef;
            }
        }


        public static List<TransacaoTEFLojas> GetLojas(long idconta)
        {
            try
            {

                var rtef = new List<TransacaoTEFLojas>();

                Leitor l = DAL.GetLeitorFromSQL(@"SELECT 
                                                    distinct """" as loja
                                                FROM
                                                    cadastro_transacoes_tef
                                                union all
                                                SELECT
                                                     distinct loja
                                                  FROM 
                                                      cadastro_transacoes_tef where " + String.Format(@"id_conta = {0}", idconta)
                                                 );
                while (!l.Eof)
                {
                    rtef.Add(new TransacaoTEFLojas()
                    {

                        loja = l.GetString("loja")

                    });
                    l.Next();
                }

                return rtef;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public static List<TransacaoTEFTpTransacao> GetTipoTransacoes(long idconta)
        {
            try
            {

                var rtef = new List<TransacaoTEFTpTransacao>();

                Leitor l = DAL.GetLeitorFromSQL(@"SELECT 
                                                    distinct """" as transacao_pagamento
                                                FROM
                                                    cadastro_transacoes_tef
                                                union all
                                                SELECT
                                                     distinct transacao_pagamento
                                                  FROM 
                                                      cadastro_transacoes_tef where " + String.Format(@"id_conta = {0}", idconta)
                                                 );
                while (!l.Eof)
                {
                    rtef.Add(new TransacaoTEFTpTransacao()
                    {

                        tp_transacao = l.GetString("transacao_pagamento")

                    });
                    l.Next();
                }

                return rtef;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public static List<TransacaoTEFListar> TransacaoTEFListar(long idconta, string rede, long tpsituacao, long tpdata, long datainicio, long datafinal, decimal valor, string estabelecimento, string administrador, string resumo, string loja, string tp_transacao, string tp_operacao)
        {
            try
            {

                var tef = DAL.GetObjeto<BancoTEF>(string.Format("id_conta ={0}", idconta));

                var nome_cliente =
                    DAL.GetString(string.Format("select nm_tef from sistema_conta where id={0}", idconta), "erro");

                var rtef = new List<TransacaoTEFListar>();
                //  "Data Source=jdbc:oracle:thin:@172.31.255.20:1521:xe;User Id=vspague_Matheusnext;Password=administrador123#"

                // server
                /*string str_con =
                    string.Format(
                        @"User Id={0};Password=""{1}"";Data Source=(DESCRIPTION =" +
                        "(ADDRESS = (PROTOCOL = TCP)(HOST = {2})(PORT = {3}))" + "(CONNECT_DATA =" +
                        "(SERVER = DEDICATED)" + "(SERVICE_NAME = XE)))", tef.usuario_banco, tef.senha_banco,
                        tef.ip_banco, tef.porta_banco);*/

                ////////using (ConexaoOracle conexao = ConexaoOracle.Get(str_con))

                // local    
                // using (ConexaoOracle conexao = ConexaoOracle.Get(@"User Id=vspague_suporte;Password=""@s0t3cht1"";Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)(HOST = 172.31.255.20)(PORT = 1521))" + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = XE)))"))
                ////{
                int parcela;
                decimal vlbruto;
                DateTime dttransacao;

                string sql = "";

                /*   String.Format(@"select Estabelecimento, Transacao_Inicio, Transacao_Valor, Transacao_Parcela, 
                   Sequencial, Transacao_Nsu, Transacao_Nsu_Rede,Transacao_Autorizacao, 
                   Transacao_Identificacao, transacao_situacao, rede from VALOR_LOJA_TRANSACAO_V775 
                   where UPPER(estabelecimento) like '{3}%' and TIPO_TRANSACAO = 'Cartão Vender' and TO_DATE(Transacao_Inicio) BETWEEN TO_DATE('{0}','yyyy/mm/dd') and  TO_DATE('{1}','yyyy/mm/dd') {2} ", new DateTime(datainicio).ToString("yyyy-MM-dd"), new DateTime(datafinal).ToString("yyyy-MM-dd"), (rede.Equals("") || rede.Equals("000") || rede.Equals("\"\"")) ? "" : string.Format("and UPPER(rede) like '%{0}%' ", rede), nome_cliente); */

                if (valor > 0)
                {
                    sql = sql + string.Format(" and cast(transacao_valor as DECIMAL(12,2)) = {0} ", valor);
                }

                if (!estabelecimento.Equals("-111") && (!estabelecimento.Equals("\"\"")))
                {
                    sql = sql + string.Format(" and UPPER(estabelecimento) = '{0}' ", estabelecimento.ToUpper());
                }

                if (!administrador.Equals("-111") && (!administrador.Equals("\"\"")))
                {
                    sql = sql + string.Format(" and UPPER(administrador) = '{0}' ", administrador.ToUpper());
                }

                if (!string.IsNullOrEmpty(resumo) && (!resumo.Equals("0")))
                {
                    sql = sql + string.Format(" and ((cast(transacao_nsu as UNSIGNED ) = {0}) or (cast(Transacao_Nsu_Rede as UNSIGNED ) = {0})) ", resumo.ToUpper());
                }

                if (!string.IsNullOrEmpty(loja) && (!loja.Equals("0")))
                {
                    sql = sql + string.Format(" and upper(loja) = upper('{0}') ", loja.ToUpper());
                }

                if (!string.IsNullOrEmpty(tp_transacao) && (!tp_transacao.Equals("0")))
                {
                    sql = sql + string.Format(" and upper(transacao_pagamento) = upper('{0}') ", tp_transacao.ToUpper());
                }

                if (!string.IsNullOrEmpty(tp_operacao) && (!tp_operacao.Equals("0")))
                {
                    sql = sql + string.Format(" and upper(tipo_cartao) = upper('{0}') ", tp_operacao.ToUpper());
                }
                
                Leitor l = DAL.GetLeitorFromSQL(@"SELECT 
                                                          sequencial,
                                                          data_atual,
                                                          estabelecimento,
                                                          loja,
                                                          terminal,
                                                          terminal_validade,
                                                          rede,
                                                          tipo_cartao,
                                                          administrador,
                                                          tipo_transacao,
                                                          produto,
                                                          cartao_bin,
                                                          cartao_numero,
                                                          cartao_validade,
                                                          cartao_entrada,
                                                          transacao_inicio,
                                                          transacao_fim,
                                                          transacao_conclusao,
                                                          transacao_pagamento,
                                                          transacao_financiado,
                                                          erro,
                                                          transacao_identificacao,
                                                          transacao_nsu,
                                                          transacao_nsu_rede,
                                                          transacao_valor,
                                                          transacao_parcela,
                                                          transacao_autorizacao,
                                                          transacao_resposta,
                                                          transacao_situacao,
                                                          id_conta FROM 
                                                          cadastro_transacoes_tef where " + String.Format(@"id_conta = {3} and cast(Transacao_Inicio as DATETIME) BETWEEN '{0} 00:00:00' and '{1} 23:59:59' {2} ", new DateTime(datainicio).ToString("yyyy-MM-dd"), new DateTime(datafinal).ToString("yyyy-MM-dd"), (rede.Trim().Equals("") || rede.Trim().Equals("000") || rede.Trim().Equals("\"\"")) ? "" : string.Format("and UPPER(rede) like '%{0}%' ", rede.Trim()), idconta)
                                                              + sql);


                // List <TransacoesTEF> TrTEF = DAL.ListarObjetos<TransacoesTEF>(String.Format(@"id_conta = {3} and TIPO_TRANSACAO = 'Cartão Vender' and cast(Transacao_Inicio as DATETIME) BETWEEN '{0} 00:00:00' and '{1} 23:59:59' {2} ", new DateTime(datainicio).ToString("yyyy-MM-dd"), new DateTime(datafinal).ToString("yyyy-MM-dd"), (rede.Equals("") || rede.Equals("000") || rede.Equals("\"\"")) ? "" : string.Format("and UPPER(rede) like '%{0}%' ", rede), idconta)
                //                                               + sql);


                /*  using (ComandoOracle comando = new ComandoOracle(conexao, sql))
                  {
                      var retorno = comando.Select(sql);

                      while (retorno.Read())
                      { */
                while (!l.Eof)
                {
                    ///    {
                    ///       System.Console.WriteLine(l.GetString("cd_funcionario") + "-" + l.GetString("nome"));
                    ///     
                    ///     }



                    rtef.Add(new TransacaoTEFListar()
                    {

                        //select 1 from `conciliador_userede_eevc_comprovantevenda` a,`conciliador_userede_eevd_comprovantevenda` b where ((a.numero_cv = {0} ) or (b.numero_cv = {0} )) and ((a.id_conta = {1} ) and (b.id_conta = {1} ))

                        /*conciliado = Convert.ToInt32(DAL.GetInt(string.Format(@"select distinct encontrou
                                                                                 from(
                                                                                 select 1 as encontrou
                                                                                 from
                                                                                     conciliador_userede_eevd_comprovantevenda b
                                                                                 where
                                                                                     (b.numero_cv = cast('{0}' as DECIMAL)) and (b.id_conta = {1} )
                                                                                 union all
                                                                                 select
                                                                                     1
                                                                                 from
                                                                                     conciliador_userede_eevc_comprovantevenda c
                                                                                 where
                                                                                  (c.numero_cv = cast('{0}' as DECIMAL)) and (c.id_conta = {1} )  ) as x  )", retorno[6].ToString(),idconta),0)), */
                        /* nm_estabelecimento = retorno[0].ToString().ToUpper(),
                         dt_transacao = DateTime.TryParse(retorno[1].ToString(), out dttransacao) ? dttransacao : DateTime.Now,
                         vl_bruto = decimal.TryParse(retorno[2].ToString(), out vlbruto) ? vlbruto : 0,
                         tot_parcela = int.TryParse(retorno[3].ToString(), out parcela) ? parcela : 0,
                         codigo_tef = retorno[4].ToString(),
                         nsu_tef = retorno[5].ToString(),
                         nsu_rede = retorno[6].ToString(),
                         is_autorizacao = retorno[7].ToString(),
                         nr_logico = retorno[8].ToString(),
                         transacao_situacao = retorno[9].ToString(),
                         ds_rede = retorno[10].ToString().ToUpper() */

                        nm_estabelecimento = l.GetString("estabelecimento"),
                        dt_transacao = DateTime.TryParse(l.GetString("transacao_inicio"), out dttransacao) ? dttransacao : DateTime.Now,
                        vl_bruto = decimal.TryParse(l.GetString("transacao_valor").Replace(",", "").Replace(".", ","), out vlbruto) ? vlbruto : 0,
                        tot_parcela = Convert.ToInt16(l.GetString("transacao_parcela").Equals("") ? "0" : l.GetString("transacao_parcela") ) == 0 ? 1 : Convert.ToInt16(l.GetString("transacao_parcela")),
                        codigo_tef = l.GetString("sequencial"),
                        nsu_tef = l.GetString("transacao_nsu"),
                        nsu_rede = l.GetString("transacao_nsu_rede"),
                        is_autorizacao = l.GetString("transacao_autorizacao"),
                        nr_logico = l.GetString("transacao_identificacao"),
                        transacao_situacao = l.GetString("transacao_situacao"),
                        ds_rede = l.GetString("rede"),
                        transacao_pagamento = l.GetString("transacao_pagamento"),
                        loja = l.GetString("loja"),
                        tipo_cartao = l.GetString("tipo_cartao"),
                        administrador = l.GetString("administrador"),
                        terminal = l.GetString("terminal")



                    });
                    l.Next();
                }

                /*      }
                  } */
                /// }
                return rtef;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public static TransacoesTEF TransacaoIncluir(long idconta, long datainicio)
        {
            try
            {
                var tef = DAL.GetObjeto<BancoTEF>(string.Format("id_conta ={0}", idconta));

                var nome_cliente =
                    DAL.GetString(string.Format("select nm_tef from sistema_conta where id={0}", idconta), "erro");

                var rtef = new TransacoesTEF();
                //  "Data Source=jdbc:oracle:thin:@172.31.255.20:1521:xe;User Id=vspague_Matheusnext;Password=administrador123#"

                // server
                string str_con =
                    string.Format(
                        @"User Id={0};Password=""{1}"";Data Source=(DESCRIPTION =" +
                        "(ADDRESS = (PROTOCOL = TCP)(HOST = {2})(PORT = {3}))" + "(CONNECT_DATA =" +
                        "(SERVER = DEDICATED)" + "(SERVICE_NAME = XE)))", tef.usuario_banco, tef.senha_banco,
                        tef.ip_banco, tef.porta_banco);

                using (ConexaoOracle conexao = ConexaoOracle.Get(str_con))

                // local    
                // using (ConexaoOracle conexao = ConexaoOracle.Get(@"User Id=vspague_suporte;Password=""@s0t3cht1"";Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)(HOST = 172.31.255.20)(PORT = 1521))" + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = XE)))"))
                {
                    string sql =
                    String.Format(@"select SEQUENCIAL,DATA_ATUAL,ESTABELECIMENTO,LOJA,TERMINAL,TERMINAL_VALIDADE,REDE,TIPO_CARTAO,
                                           ADMINISTRADOR,TIPO_TRANSACAO,PRODUTO,CARTAO_BIN,CARTAO_NUMERO,CARTAO_VALIDADE,CARTAO_ENTRADA,TRANSACAO_INICIO,TRANSACAO_FIM,TRANSACAO_CONCLUSAO,
                                           TRANSACAO_PAGAMENTO,TRANSACAO_FINANCIADO,ERRO,TRANSACAO_IDENTIFICACAO,TRANSACAO_NSU,TRANSACAO_NSU_REDE,TRANSACAO_VALOR,TRANSACAO_PARCELA,TRANSACAO_AUTORIZACAO,
                                           TRANSACAO_RESPOSTA,TRANSACAO_SITUACAO 
                                           from VALOR_LOJA_TRANSACAO_V775 
                                           where UPPER(TRANSACAO_SITUACAO)='COM SUCESSO' and UPPER(estabelecimento) like UPPER('{0}%') and TIPO_TRANSACAO = 'Cartão Vender' and TO_DATE(Transacao_Inicio) >= TO_DATE('{1}','yyyy/mm/dd') and TO_DATE(Transacao_Inicio) < TRUNC(sysdate) order by DATA_ATUAL ", nome_cliente, new DateTime(datainicio).ToString("yyyy-MM-dd"));

                    using (ComandoOracle comando = new ComandoOracle(conexao, sql))
                    {
                        OracleDataReader retorno = comando.Select(sql);
                        int count = 0;
                        StringBuilder str = new StringBuilder();
                        while (retorno.Read())
                        {
                            /*
                            str.AppendLine(" " + count + " - " + retorno.GetName(count) + " -> " + retorno.GetFieldType(count));
                            count++;*/

                            try
                            {
                                var tra = new TransacoesTEF(); //DAL.GetObjeto<TransacoesTEF>(string.Format("sequencial = '{0}' ", retorno[0].ToString())) ?? new TransacoesTEF();
                                tra.sequencial = retorno.GetValue(0) != DBNull.Value ? retorno.GetInt64(0).ToString() : "";
                                tra.data_atual = retorno.GetValue(1) != DBNull.Value ? retorno.GetDateTime(1) : DateTime.MinValue;
                                tra.estabelecimento = retorno.GetValue(2) != DBNull.Value ? retorno.GetString(2) : "";
                                tra.loja = retorno.GetValue(3) != DBNull.Value ? retorno.GetString(3) : "";
                                tra.terminal = retorno.GetValue(4) != DBNull.Value ? retorno.GetString(4) : "";
                                tra.terminal_validade = retorno.GetValue(5) != DBNull.Value ? retorno.GetDateTime(5) : DateTime.MinValue;
                                tra.rede = retorno.GetValue(6) != DBNull.Value ? retorno.GetString(6) : "";
                                tra.tipo_cartao = retorno.GetValue(7) != DBNull.Value ? retorno.GetString(7) : "";
                                tra.administrador = retorno.GetValue(8) != DBNull.Value ? retorno.GetString(8) : "";
                                tra.tipo_transacao = retorno.GetValue(9) != DBNull.Value ? retorno.GetString(9) : "";
                                tra.produto = retorno.GetValue(10) != DBNull.Value ? retorno.GetString(10) : "";
                                tra.cartao_bin = retorno.GetValue(11) != DBNull.Value ? retorno.GetInt64(11) : 0;
                                tra.cartao_numero = retorno.GetValue(12) != DBNull.Value ? retorno.GetInt64(12) : 0;
                                tra.cartao_validade = retorno.GetValue(13) != DBNull.Value ? retorno.GetDateTime(13) : DateTime.MinValue;
                                tra.cartao_entrada = retorno.GetValue(14) != DBNull.Value ? retorno.GetString(14) : "";
                                tra.transacao_inicio = retorno.GetValue(15) != DBNull.Value ? retorno.GetDateTime(15) : DateTime.MinValue;
                                tra.transacao_fim = retorno.GetValue(16) != DBNull.Value ? retorno.GetDateTime(16) : DateTime.MinValue;
                                tra.transacao_conclusao = retorno.GetValue(17) != DBNull.Value ? retorno.GetDateTime(17) : DateTime.MinValue;
                                tra.transacao_pagamento = retorno.GetValue(18) != DBNull.Value ? retorno.GetString(18) : "";
                                tra.transacao_financiado = retorno.GetValue(19) != DBNull.Value ? retorno.GetString(19) : "";
                                tra.erro = retorno.GetValue(20) != DBNull.Value ? retorno.GetString(20) : "";
                                tra.transacao_identificacao = retorno.GetValue(21) != DBNull.Value ? retorno.GetString(21) : "";
                                tra.transacao_nsu = retorno.GetValue(22) != DBNull.Value ? retorno.GetInt64(22) : 0;
                                tra.transacao_nsu_rede = retorno.GetValue(23) != DBNull.Value ? retorno.GetInt64(23) : 0;
                                tra.transacao_valor = retorno.GetValue(24) != DBNull.Value ? retorno.GetDecimal(24) : 0;
                                tra.transacao_parcela = retorno.GetValue(25) != DBNull.Value ? retorno.GetInt16(25) : 0;
                                tra.transacao_autorizacao = retorno.GetValue(26) != DBNull.Value ? retorno.GetString(26) : "";
                                tra.transacao_resposta = retorno.GetValue(27) != DBNull.Value ? retorno.GetString(27) : "";
                                tra.transacao_situacao = retorno.GetValue(28) != DBNull.Value ? retorno.GetString(28) : "";
                                tra.IdConta = idconta;
                                DAL.Gravar(tra);
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                    }
                }

                return rtef;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public static TransacoesTEF TransacaoIncluir(long idconta, long datainicio, BancoTEF tef)
        {
            try
            {
                //var tef = DAL.GetObjeto<BancoTEF>(string.Format("id_conta ={0}", idconta));

                // var nome_cliente =
                //    DAL.GetString(string.Format("select nm_tef from sistema_conta where id={0}", idconta), "erro");

                List<Estabelecimento> Estabelecimentos = DAL.ListarObjetos<Estabelecimento>(string.Format("id_conta={0} and is_sincroniza_tef=1", tef.IdConta));
                string CNPJS = "";
                if (Estabelecimentos.Count > 0) {
                    CNPJS = Estabelecimentos.Select(s => (string)s.cpfcnpj.Replace("/", "").Replace("-", "").Replace(".", "")).ToList().Aggregate((current, next) => current + "," + next);
                }
                

                var rtef = new TransacoesTEF();
                //  "Data Source=jdbc:oracle:thin:@172.31.255.20:1521:xe;User Id=vspague_Matheusnext;Password=administrador123#"

                // server
                string str_con =
                    string.Format(
                        @"User Id={0};Password=""{1}"";Data Source=(DESCRIPTION =" +
                        "(ADDRESS = (PROTOCOL = TCP)(HOST = {2})(PORT = {3}))" + "(CONNECT_DATA =" +
                        "(SERVER = DEDICATED)" + "(SERVICE_NAME = XE)))", tef.usuario_banco, tef.senha_banco,
                        tef.ip_banco, tef.porta_banco);

                string sql =
                    String.Format(@"select p.SEQUENCIAL,p.DATA_ATUAL,p.ESTABELECIMENTO,p.LOJA,p.TERMINAL,p.TERMINAL_VALIDADE,p.REDE,p.TIPO_CARTAO,
                                           p.ADMINISTRADOR,TIPO_TRANSACAO,p.PRODUTO,p.CARTAO_BIN,p.CARTAO_NUMERO,p.CARTAO_VALIDADE,p.CARTAO_ENTRADA,p.TRANSACAO_INICIO,p.TRANSACAO_FIM,p.TRANSACAO_CONCLUSAO,
                                           p.TRANSACAO_PAGAMENTO,p.TRANSACAO_FINANCIADO,p.ERRO,p.TRANSACAO_IDENTIFICACAO,p.TRANSACAO_NSU,p.TRANSACAO_NSU_REDE,p.TRANSACAO_VALOR,p.TRANSACAO_PARCELA,p.TRANSACAO_AUTORIZACAO,
                                           p.TRANSACAO_RESPOSTA,p.TRANSACAO_SITUACAO 
                                           from VALOR_LOJA_TRANSACAO_V775 p 
                                           join valor_loja_v8 v on v.loja = p.loja and p.estabelecimento = v.estabelecimento   
                                           where UPPER(p.TRANSACAO_SITUACAO)='COM SUCESSO' and UPPER(p.estabelecimento) like UPPER('{0}%') and p.TIPO_TRANSACAO = 'Cartão Vender' and TO_DATE(p.Transacao_Inicio) >= TO_DATE('{1}','yyyy/mm/dd') and TO_DATE(p.Transacao_Inicio) < TRUNC(sysdate) ", tef.identificacao_tef, new DateTime(datainicio).ToString("yyyy-MM-dd"));

                if (!string.IsNullOrEmpty(CNPJS)) {
                    sql += string.Format(" and CAST(regexp_replace(v.CNPJ_CPF, '[^[:digit:]]') as INT) in ({0}) ", CNPJS);
                }
                sql += " order by DATA_ATUAL ";

                using (ConexaoOracle conexao = ConexaoOracle.Get(str_con))

                // local    
                // using (ConexaoOracle conexao = ConexaoOracle.Get(@"User Id=vspague_suporte;Password=""@s0t3cht1"";Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)(HOST = 172.31.255.20)(PORT = 1521))" + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = XE)))"))
                {
                    

                    using (ComandoOracle comando = new ComandoOracle(conexao, sql))
                    {
                        OracleDataReader retorno = comando.Select(sql);
                        int count = 0;
                        StringBuilder str = new StringBuilder();
                        while (retorno.Read())
                        {
                            /*
                            str.AppendLine(" " + count + " - " + retorno.GetName(count) + " -> " + retorno.GetFieldType(count));
                            count++;*/

                            try
                            {
                                var tra = new TransacoesTEF(); //DAL.GetObjeto<TransacoesTEF>(string.Format("sequencial = '{0}' ", retorno[0].ToString())) ?? new TransacoesTEF();
                                tra.sequencial = retorno.GetValue(0) != DBNull.Value ? retorno.GetInt64(0).ToString() : "";
                                tra.data_atual = retorno.GetValue(1) != DBNull.Value ? retorno.GetDateTime(1) : DateTime.MinValue;
                                tra.estabelecimento = retorno.GetValue(2) != DBNull.Value ? retorno.GetString(2) : "";
                                tra.loja = retorno.GetValue(3) != DBNull.Value ? retorno.GetString(3) : "";
                                tra.terminal = retorno.GetValue(4) != DBNull.Value ? retorno.GetString(4) : "";
                                tra.terminal_validade = retorno.GetValue(5) != DBNull.Value ? retorno.GetDateTime(5) : DateTime.MinValue;
                                tra.rede = retorno.GetValue(6) != DBNull.Value ? retorno.GetString(6) : "";
                                tra.tipo_cartao = retorno.GetValue(7) != DBNull.Value ? retorno.GetString(7) : "";
                                tra.administrador = retorno.GetValue(8) != DBNull.Value ? retorno.GetString(8) : "";
                                tra.tipo_transacao = retorno.GetValue(9) != DBNull.Value ? retorno.GetString(9) : "";
                                tra.produto = retorno.GetValue(10) != DBNull.Value ? retorno.GetString(10) : "";
                                tra.cartao_bin = retorno.GetValue(11) != DBNull.Value ? retorno.GetInt64(11) : 0;
                                tra.cartao_numero = retorno.GetValue(12) != DBNull.Value ? retorno.GetInt64(12) : 0;
                                tra.cartao_validade = retorno.GetValue(13) != DBNull.Value ? retorno.GetDateTime(13) : DateTime.MinValue;
                                tra.cartao_entrada = retorno.GetValue(14) != DBNull.Value ? retorno.GetString(14) : "";
                                tra.transacao_inicio = retorno.GetValue(15) != DBNull.Value ? retorno.GetDateTime(15) : DateTime.MinValue;
                                tra.transacao_fim = retorno.GetValue(16) != DBNull.Value ? retorno.GetDateTime(16) : DateTime.MinValue;
                                tra.transacao_conclusao = retorno.GetValue(17) != DBNull.Value ? retorno.GetDateTime(17) : DateTime.MinValue;
                                tra.transacao_pagamento = retorno.GetValue(18) != DBNull.Value ? retorno.GetString(18) : "";
                                tra.transacao_financiado = retorno.GetValue(19) != DBNull.Value ? retorno.GetString(19) : "";
                                tra.erro = retorno.GetValue(20) != DBNull.Value ? retorno.GetString(20) : "";
                                tra.transacao_identificacao = retorno.GetValue(21) != DBNull.Value ? retorno.GetString(21) : "";
                                tra.transacao_nsu = retorno.GetValue(22) != DBNull.Value ? retorno.GetInt64(22) : 0;
                                tra.transacao_nsu_rede = retorno.GetValue(23) != DBNull.Value ? retorno.GetInt64(23) : 0;
                                tra.transacao_valor = retorno.GetValue(24) != DBNull.Value ? retorno.GetDecimal(24) : 0;
                                tra.transacao_parcela = retorno.GetValue(25) != DBNull.Value ? retorno.GetInt16(25) : 0;
                                tra.transacao_autorizacao = retorno.GetValue(26) != DBNull.Value ? retorno.GetString(26) : "";
                                tra.transacao_resposta = retorno.GetValue(27) != DBNull.Value ? retorno.GetString(27) : "";
                                tra.transacao_situacao = retorno.GetValue(28) != DBNull.Value ? retorno.GetString(28) : "";
                                tra.IdConta = idconta;
                                DAL.Gravar(tra);
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                    }
                }

                return rtef;
            }
            catch (Exception e)
            {
                return new TransacoesTEF();//throw e;
            }

        }

        public static TransacoesTEF TransacaoIncluir(long idconta, long datainicio, long datafinal, BancoTEF tef)
        {
            try
            {
                //var tef = DAL.GetObjeto<BancoTEF>(string.Format("id_conta ={0}", idconta));

                // var nome_cliente =
                //    DAL.GetString(string.Format("select nm_tef from sistema_conta where id={0}", idconta), "erro");

                var rtef = new TransacoesTEF();
                //  "Data Source=jdbc:oracle:thin:@172.31.255.20:1521:xe;User Id=vspague_Matheusnext;Password=administrador123#"

                // server
                string str_con =
                    string.Format(
                        @"User Id={0};Password=""{1}"";Data Source=(DESCRIPTION =" +
                        "(ADDRESS = (PROTOCOL = TCP)(HOST = {2})(PORT = {3}))" + "(CONNECT_DATA =" +
                        "(SERVER = DEDICATED)" + "(SERVICE_NAME = XE)))", tef.usuario_banco, tef.senha_banco,
                        tef.ip_banco, tef.porta_banco);

                using (ConexaoOracle conexao = ConexaoOracle.Get(str_con))

                // local    
                // using (ConexaoOracle conexao = ConexaoOracle.Get(@"User Id=vspague_suporte;Password=""@s0t3cht1"";Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)(HOST = 172.31.255.20)(PORT = 1521))" + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = XE)))"))
                {
                    string sql =
                    String.Format(@"select SEQUENCIAL,DATA_ATUAL,ESTABELECIMENTO,LOJA,TERMINAL,TERMINAL_VALIDADE,REDE,TIPO_CARTAO,
                                           ADMINISTRADOR,TIPO_TRANSACAO,PRODUTO,CARTAO_BIN,CARTAO_NUMERO,CARTAO_VALIDADE,CARTAO_ENTRADA,TRANSACAO_INICIO,TRANSACAO_FIM,TRANSACAO_CONCLUSAO,
                                           TRANSACAO_PAGAMENTO,TRANSACAO_FINANCIADO,ERRO,TRANSACAO_IDENTIFICACAO,TRANSACAO_NSU,TRANSACAO_NSU_REDE,TRANSACAO_VALOR,TRANSACAO_PARCELA,TRANSACAO_AUTORIZACAO,
                                           TRANSACAO_RESPOSTA,TRANSACAO_SITUACAO 
                                           from VALOR_LOJA_TRANSACAO_V775 
                                           where UPPER(TRANSACAO_SITUACAO)='COM SUCESSO' and UPPER(estabelecimento) like UPPER('{0}%') and TIPO_TRANSACAO = 'Cartão Vender' and TO_DATE(Transacao_Inicio) >= TO_DATE('{1}','yyyy/mm/dd') and TO_DATE(Transacao_Inicio) < TO_DATE('{2}','yyyy/mm/dd') order by DATA_ATUAL ", tef.identificacao_tef, new DateTime(datainicio).ToString("yyyy-MM-dd"), new DateTime(datafinal).ToString("yyyy-MM-dd"));

                    using (ComandoOracle comando = new ComandoOracle(conexao, sql))
                    {
                        OracleDataReader retorno = comando.Select(sql);
                        int count = 0;
                        StringBuilder str = new StringBuilder();
                        while (retorno.Read())
                        {
                            /*
                            str.AppendLine(" " + count + " - " + retorno.GetName(count) + " -> " + retorno.GetFieldType(count));
                            count++;*/

                            try
                            {
                                var tra = new TransacoesTEF(); //DAL.GetObjeto<TransacoesTEF>(string.Format("sequencial = '{0}' ", retorno[0].ToString())) ?? new TransacoesTEF();
                                tra.sequencial = retorno.GetValue(0) != DBNull.Value ? retorno.GetInt64(0).ToString() : "";
                                tra.data_atual = retorno.GetValue(1) != DBNull.Value ? retorno.GetDateTime(1) : DateTime.MinValue;
                                tra.estabelecimento = retorno.GetValue(2) != DBNull.Value ? retorno.GetString(2) : "";
                                tra.loja = retorno.GetValue(3) != DBNull.Value ? retorno.GetString(3) : "";
                                tra.terminal = retorno.GetValue(4) != DBNull.Value ? retorno.GetString(4) : "";
                                tra.terminal_validade = retorno.GetValue(5) != DBNull.Value ? retorno.GetDateTime(5) : DateTime.MinValue;
                                tra.rede = retorno.GetValue(6) != DBNull.Value ? retorno.GetString(6) : "";
                                tra.tipo_cartao = retorno.GetValue(7) != DBNull.Value ? retorno.GetString(7) : "";
                                tra.administrador = retorno.GetValue(8) != DBNull.Value ? retorno.GetString(8) : "";
                                tra.tipo_transacao = retorno.GetValue(9) != DBNull.Value ? retorno.GetString(9) : "";
                                tra.produto = retorno.GetValue(10) != DBNull.Value ? retorno.GetString(10) : "";
                                tra.cartao_bin = retorno.GetValue(11) != DBNull.Value ? retorno.GetInt64(11) : 0;
                                tra.cartao_numero = retorno.GetValue(12) != DBNull.Value ? retorno.GetInt64(12) : 0;
                                tra.cartao_validade = retorno.GetValue(13) != DBNull.Value ? retorno.GetDateTime(13) : DateTime.MinValue;
                                tra.cartao_entrada = retorno.GetValue(14) != DBNull.Value ? retorno.GetString(14) : "";
                                tra.transacao_inicio = retorno.GetValue(15) != DBNull.Value ? retorno.GetDateTime(15) : DateTime.MinValue;
                                tra.transacao_fim = retorno.GetValue(16) != DBNull.Value ? retorno.GetDateTime(16) : DateTime.MinValue;
                                tra.transacao_conclusao = retorno.GetValue(17) != DBNull.Value ? retorno.GetDateTime(17) : DateTime.MinValue;
                                tra.transacao_pagamento = retorno.GetValue(18) != DBNull.Value ? retorno.GetString(18) : "";
                                tra.transacao_financiado = retorno.GetValue(19) != DBNull.Value ? retorno.GetString(19) : "";
                                tra.erro = retorno.GetValue(20) != DBNull.Value ? retorno.GetString(20) : "";
                                tra.transacao_identificacao = retorno.GetValue(21) != DBNull.Value ? retorno.GetString(21) : "";
                                tra.transacao_nsu = retorno.GetValue(22) != DBNull.Value ? retorno.GetInt64(22) : 0;
                                tra.transacao_nsu_rede = retorno.GetValue(23) != DBNull.Value ? retorno.GetInt64(23) : 0;
                                tra.transacao_valor = retorno.GetValue(24) != DBNull.Value ? retorno.GetDecimal(24) : 0;
                                tra.transacao_parcela = retorno.GetValue(25) != DBNull.Value ? retorno.GetInt16(25) : 0;
                                tra.transacao_autorizacao = retorno.GetValue(26) != DBNull.Value ? retorno.GetString(26) : "";
                                tra.transacao_resposta = retorno.GetValue(27) != DBNull.Value ? retorno.GetString(27) : "";
                                tra.transacao_situacao = retorno.GetValue(28) != DBNull.Value ? retorno.GetString(28) : "";
                                tra.IdConta = idconta;
                                DAL.Gravar(tra);
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                    }
                }

                return rtef;
            }
            catch (Exception e)
            {
                throw e;
            }

        }


    }
}
