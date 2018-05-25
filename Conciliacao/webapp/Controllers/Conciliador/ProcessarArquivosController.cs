using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Conciliacao.Helper.Rest;
using Conciliacao.Models;
using ConciliacaoModelo.model.cadastros;
using ConciliacaoModelo.model.conciliador;
using ConciliacaoModelo.model.generico;
using ConciliacaoPersistencia.banco;

namespace Conciliacao.Controllers.Conciliador
{
    [Authorize]
    public class ProcessarArquivosController : Controller
    {
        static readonly RedeRestClient _restRede = new RedeRestClient();
        static readonly VansRestClient _restVan = new VansRestClient();

        [AllowAnonymous]
        public Boolean CriarPastasCielo(string Conta)
        {
            var path = string.Format("~/ARQUIVOS/{0}/{1}", Conta, "CIELO");
            var map = Server.MapPath(path);

            Directory.CreateDirectory(@map);
            Directory.CreateDirectory(@map + "\\PROCESSADOS");
            Directory.CreateDirectory(@map + "\\SEM_REGISTRO");
            Directory.CreateDirectory(@map + "\\PROBLEMAS");
            return true;
        }


        // GET: ProcessarArquivos
        [AllowAnonymous]
        public Boolean ProcessarArquivosCielo(string Conta)
        {

            try
            {

                var path = string.Format("~/ARQUIVOS/{0}/{1}", Conta, "CIELO");
                var map = Server.MapPath(path);

                System.IO.Directory.CreateDirectory(@map);

                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(@map);
                foreach (System.IO.FileInfo f in di.GetFiles())
                {
                    using (var fileStream = new FileStream(@map + "\\" + f.Name, FileMode.Open))
                    {
                        string processar = Processar(fileStream);
                        fileStream.Close();
                        if (processar.Equals("processado"))
                        {
                            System.IO.Directory.CreateDirectory(@map + "\\PROCESSADOS");
                            System.IO.File.Move(@map + "\\" + f.Name, @map + "\\PROCESSADOS" + "\\" + f.Name);
                        }
                        else if (processar.Equals("semregistro"))
                        {
                            System.IO.Directory.CreateDirectory(@map + "\\SEM_REGISTRO");
                            System.IO.File.Move(@map + "\\" + f.Name, @map + "\\SEM_REGISTRO" + "\\" + f.Name);
                        }
                        else
                        {
                            System.IO.Directory.CreateDirectory(@map + "\\PROBLEMAS");
                            System.IO.File.Move(@map + "\\" + f.Name, @map + "\\PROBLEMAS" + "\\" + f.Name);
                        }
                    }
                }



            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }




        // GET: ProcessarArquivos
        /*   [AllowAnonymous]
           public Boolean Index()
           {

               IEnumerable<RedeListar> redes = _restRede.GetRedesAll("");

               foreach (var item in redes)
               {
                   if (!item.HabilitaConciliacaoAutomatica) continue;
                   var vans = _restVan.GetVansAll(string.Format("id_rede ={0}", item.id_rede));
                   foreach (var itemvan in vans)
                   {

                       try
                       {
                           var credentials = new NetworkCredential(itemvan.usuario_rede, itemvan.senha_rede);
                           var url = "ftp://" + itemvan.rede_ftp + ":" + itemvan.rede_porta.ToUpper() + "/" + itemvan.dir_base_arquivos_ftp;

                           var path = string.Format("~/ARQUIVOS/{0}", item.Nome);
                           var map = Server.MapPath(path);

                           DownloadFtpDirectory(url, credentials, @map);

                           System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(@map);
                           foreach (System.IO.FileInfo f in di.GetFiles())
                           {
                               using (var fileStream = new FileStream(@map + "\\" + f.Name, FileMode.Open))
                               {
                                   string processar = Processar(fileStream);
                                   fileStream.Close();
                                   if (processar.Equals("processado"))
                                   {
                                       System.IO.File.Move(@map + "\\" + f.Name, @map + "\\PROCESSADO" + "\\" + f.Name);
                                   }
                                   else if (processar.Equals("semregistro"))
                                   {
                                       System.IO.File.Move(@map + "\\" + f.Name, @map + "\\SEM_REGISTRO" + "\\" + f.Name);
                                   }
                                   else
                                   {
                                       System.IO.File.Move(@map + "\\" + f.Name, @map + "\\PROBLEMAS" + "\\" + f.Name);
                                   }
                               }
                           }



                       }
                       catch (Exception e)
                       {
                       }


                   }
               }
               return true;
           } */

        [AllowAnonymous]
        public void DownloadFtpDirectory(string url, NetworkCredential credentials, string localPath)
        {
            var listRequest = (FtpWebRequest)WebRequest.Create(url);
            listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            listRequest.Credentials = credentials;

            var lines = new List<string>();

            using (var listResponse = (FtpWebResponse)listRequest.GetResponse())
            using (var listStream = listResponse.GetResponseStream())
            using (var listReader = new StreamReader(listStream))
            {
                while (!listReader.EndOfStream)
                {
                    lines.Add(listReader.ReadLine());
                }
            }

            foreach (string line in lines)
            {
                string[] tokens =
                    line.Split(new[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                string name = tokens[3];
                //// string permissions = tokens[0];

                string localFilePath = localPath + "\\" + name;
                string fileUrl = url + "/" + name;


                /*   if (permissions[0] == 'd')
                   {
                       if (!Directory.Exists(localFilePath))
                       {
                           Directory.CreateDirectory(localFilePath);
                       }

                       DownloadFtpDirectory(fileUrl , credentials, localFilePath);
                   }
                   else
                   {*/
                FtpWebRequest downloadRequest = (FtpWebRequest)WebRequest.Create(fileUrl);
                downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                downloadRequest.Credentials = credentials;

                using (var downloadResponse =
                          (FtpWebResponse)downloadRequest.GetResponse())
                using (Stream sourceStream = downloadResponse.GetResponseStream())
                using (Stream targetStream = System.IO.File.Create(localFilePath))
                {
                    var buffer = new byte[10240];
                    int read;
                    while ((read = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        targetStream.Write(buffer, 0, read);
                    }
                }

            }
        }

        public string Processar(Stream file)
        {
            string resposta = "processado";

            if (file != null && file.Length > 0)
            {
                var arquivo = new ConciliacaoArquivoManipular(new StreamReader(file));

                String first_line = arquivo.LerLinha(true);

                try
                {
                    if ((first_line.ToUpper().Contains("CIELO")) && (arquivo.ProcessarArquivoCielo(first_line))) //Opção de extrato - saldo em aberto
                    {

                        bool md5_encontrado = false;

                         string lsArquivomd5 = "";
                         string is_linha_atual = "";
                         while ((is_linha_atual = arquivo.LerLinha()) != null)
                         {
                             lsArquivomd5 = lsArquivomd5 + is_linha_atual;
                         }

                         arquivo.Seek(0);
                         string lsretornomd5 = CriarMD5.RetornarMD5(lsArquivomd5);
                         var conta = new BaseID();
                         ArquivoMD5 md5 = DAL.GetObjeto<ArquivoMD5>(string.Format("arquivo_md5='{0}' and id_conta={1}", lsretornomd5, conta.IdConta)) ?? new ArquivoMD5();
                         if (md5.arquivo_md5.Equals(""))
                         {
                             md5.arquivo_md5 = lsretornomd5;
                             
                         }
                         else
                         {
                            // md5_encontrado = true;
                         }  

                        var cielo = new ConciliacaoCieloDesmontar(arquivo, first_line);

                        if (!md5_encontrado)
                        {

                            DAL.GravarList(cielo.GetCreditos());
                            DAL.GravarList(cielo.GetResumoEEVDOperacao());
                            DAL.GravarList(cielo.GetComprovanteEEVDVenda());

                            DAL.GravarList(cielo.GetResumoEEVCArquivo());

                            DAL.GravarList(cielo.GetComprovanteEEVCCVenda());
                            DAL.GravarList(cielo.GetParcelas());

                            DAL.GravarList(cielo.TotalizadorProdutoGet());
                            DAL.GravarList(cielo.TotalizadorBancoGet());
                            DAL.GravarList(cielo.TotalizadorEstabelecimentoGet());
                            if (md5.arquivo_md5.Equals(""))
                            {
                                DAL.Gravar(md5);
                            }
                            
                            // DAL.GravarList(cielo.RedeGet());

                        }

                    }
                    else
                    {
                        resposta = "problemas";
                    }
                }
                catch (Exception ex)
                {
                    resposta = "problemas";
                }
            }
            else
            {
                resposta = "semregistro";
            }

            return resposta;
        }


    }
}