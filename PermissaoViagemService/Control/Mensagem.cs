using PermissaoViagemService.Dal;
using PermissaoViagemService.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PermissaoViagemService.Control
{
    class Mensagem
    {
        System.Net.Mail.SmtpClient client;
        private PermissaoViagemContext db = new PermissaoViagemContext();
        SolicitacaoViagem SV;

        public Boolean HabilitadoEmail { get; set; }
        public Boolean HabilitadoSMS { get; set; }

        public Mensagem()
        {
            HabilitadoEmail = true;
            HabilitadoSMS = true;
            client = new System.Net.Mail.SmtpClient();
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["EMAIL_USER"].ToString()
                                    , ConfigurationManager.AppSettings["EMAIL_PWD"].ToString());
        }

        public void TempoViagemFinalizada(int id)
        {
            SV = Solicitacao(id).FirstOrDefault();

            ParaAprovadorTempoFinalizado();
        }

        void ParaAprovadorTempoFinalizado() {

            if (SV != null)
            {
                try
                {
                    Empregado Aprovador = db.Empregados.Find(547492);

                    String msg = "<h1> Permissão Viagem: </h1><br/><br/>" +
                                 "Oi, " + Aprovador.Nome + "<br/>" +
                                 "O Tempo de Viagem previsto da SV :" + SV.Id + " já foi finalizado, mas ainda não foi ENCERRADO no APP <br/>" +
                                 "De : " + SV.Origem.Nome + " <br/>" +
                                 "Para :  " + SV.Destino.Nome + " <br/>" +
                                 "Data de Partida :  " + SV.DataPartida.ToLocalTime().ToString() + " <br/>" +
                                 "Data de Chegada Prevista:  " + SV.DataChegadaPrevista.ToLocalTime().ToString() + " <br/>" +
                                 "Transporte : " + SV.Transporte.Nome + " <br/>" +
                                 "Solicitante : " + SV.Empregado.Nome + " <br/>" +
                                 "";

                    List<Empregado> Empregados = new List<Empregado>();
                    Empregados.Add(Aprovador);

                    Page_Load(Empregados, msg);

                    ParaViajantes("A Viagem  de  SV: " + SV.Id +" já foi finalizada ? Entre no APP Permissão Viagem para Encerrá-la ");
                }
                catch (Exception e)
                {
                    DebugLog.Logar("ParaAprovadorAnalisar()" + e.Data);
                }
            }
            else
            {
                DebugLog.Logar("Solicitação viagem NULL  Mensagem");
            }

        }

        void Page_Load(List<Empregado> Empregados, String msg)
        {

            if (HabilitadoSMS)
                SMS(Empregados, msg);

            if (HabilitadoEmail)
            {
                MailMessage mail = new MailMessage();
                mail.Sender = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["EMAIL_USER"].ToString(), "Permissão Viagem");
                mail.From = new MailAddress(ConfigurationManager.AppSettings["EMAIL_USER"].ToString(), "Permissão Viagem");

                Empregados.ForEach(x =>
                {
                    mail.To.Add(new MailAddress(x.Email, x.Nome));

                });

                mail.Subject = "Permissão Viagem";

                mail.Body = msg;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                try
                {
                    client.Send(mail);
                }
                catch (System.Exception erro)
                {
                    DebugLog.Logar("Email=>>" + erro);//trata erro
                }
                finally
                {

                    mail = null;
                }
            }

        }

        void ParaViajantes(String text)
        {
            if (SV != null)
            {
                AprovadorSolicitacao aprovadorSolicitacao = SV.AprovadorSolicitacaoId.FirstOrDefault();

                String msg = "<h1> Permissão Viagem: </h1><br/><br/>" +
                             text + " <br/>" +
                             "De : " + SV.Origem.Nome + "<br/> " +
                             "Para :  " + SV.Destino.Nome + "<br/>" +
                             "Data de Partida :  " + SV.DataPartida.ToLocalTime().ToString() + "<br/>" +
                             "Data de Chegada Prevista:  " + SV.DataChegadaPrevista.ToLocalTime().ToString() + "<br/>" +
                             "Transporte : " + SV.Transporte.Nome + "<br/>" +
                             "Solicitante : " + SV.Empregado.Nome + "<br/>" +
                             "<h2>Status de Viagem : " + aprovadorSolicitacao.Status.Nome.ToUpper() + "  -  Data: " + aprovadorSolicitacao.DataStatus.ToLocalTime().ToString() + "</h2>";
                DebugLog.Logar(msg);
                List<Empregado> Empregados = SV.ViajanteSolicitacaoId.Select(e => e.Empregado).ToList();

                Page_Load(Empregados, msg);

            }

        }

        private void SMS(List<Empregado> Empregados, String msg)
        {
            String prefixo = "55";
            String sufixo = "@oicorp.oi.com.br";

            String SMS = msg.Replace("<h1>", " ").Replace("</h1>", " ").Replace("<br/>", " ").Replace("<h2>", " ").Replace("</h2>", " ");
            DebugLog.Logar(SMS);
            try
            {
                SMSContext db = new SMSContext();

                Empregados.ForEach(x =>
                {
                    if (x.Telefone != null)
                    {

                        try
                        {
                            SmsQueue smsQueue = new SmsQueue();
                            smsQueue.Sender = ""; // ConfigurationManager.AppSettings["EMAIL_USER"].ToString();
                            smsQueue.Receiver = prefixo + x.Telefone + sufixo;
                            smsQueue.Subject = "";
                            smsQueue.Message = SMS;

                            smsQueue.AppInfo = "5500";
                            smsQueue.Hist = DateTime.Now;
                            db.SmsQueue.Add(smsQueue);

                        }
                        catch (Exception ex)
                        {
                            DebugLog.Logar(ex.Message);
                        }
                    }
                });

                db.SaveChanges();

                db.Dispose();
            }
            catch (Exception ex)
            {
                DebugLog.Logar(ex.Message);
            }
        }

        List<SolicitacaoViagem> Solicitacao(int id)
        {
            List<SolicitacaoViagem> solicitacaoViagem = db.SolicitacaoViagems.Where(x => x.Id == id)
                                                                                .Include(s => s.Destino)
                                                                                .Include(s => s.Origem)
                                                                                .Include(s => s.Transporte)
                                                                                .Include(s => s.Empregado)
                                                                                .Include(s => s.AprovadorSolicitacaoId)
                                                                                .Include(s => s.ViajanteSolicitacaoId).ToList();

            if (solicitacaoViagem.FirstOrDefault() == null)
            {
                return null;
            }


            FillObjects(solicitacaoViagem);
            return solicitacaoViagem;
        }

        private void FillObjects(List<SolicitacaoViagem> solicitacaoViagems)
        {
            solicitacaoViagems.ForEach(x =>
            {
                x.AprovadorSolicitacaoId = x.AprovadorSolicitacaoId.OrderBy(y => y.Id).Reverse().ToList();
                foreach (var aprovadorSolicitacao in x.AprovadorSolicitacaoId)
                {
                    aprovadorSolicitacao.Aprovador = db.Aprovadores.Where(y => y.Id == aprovadorSolicitacao.AprovadorId)
                    .Include(y => y.Empregado).FirstOrDefault();
                    aprovadorSolicitacao.Status = db.Status.Where(y => y.Id == aprovadorSolicitacao.StatusId).FirstOrDefault();

                }
                foreach (var viajanteSolicitacao in x.ViajanteSolicitacaoId)
                {
                    viajanteSolicitacao.Empregado = db.Empregados.Where(y => y.Id == viajanteSolicitacao.EmpregadoId).FirstOrDefault();

                }
            });
        }

    }
}
