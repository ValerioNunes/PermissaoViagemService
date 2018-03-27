using PermissaoViagemService.Dal;
using PermissaoViagemService.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissaoViagemService.Control
{
 
    class TempoViagemService
    {
        private static PermissaoViagemContext db = new PermissaoViagemContext();

        public static void  VerificarViagemFinalizadas() {

            try
            {
                List<SolicitacaoViagem> solicitacaoViagems = db.SolicitacaoViagems.Include(s => s.Destino)
                                                                                    .Include(s => s.Origem)
                                                                                    .Include(s => s.Transporte)
                                                                                    .Include(s => s.Empregado)
                                                                                    .Include(s => s.AprovadorSolicitacaoId)
                                                                                    .Include(s => s.ViajanteSolicitacaoId).ToList();
                FillObjects(solicitacaoViagems);



                solicitacaoViagems = solicitacaoViagems.Where(x => x.AprovadorSolicitacaoId.FirstOrDefault().Status.Nome.Equals("viajando") &&
                                                                   x.DataChegadaPrevista < DateTime.Now).ToList();


                Mensagem mensagem = new Mensagem();
                solicitacaoViagems.ForEach(x => mensagem.TempoViagemFinalizada(x.Id));
               

            }
            catch (Exception e){
                DebugLog.Logar("Erro em  VerificarViagemFinalizadas() " + e.Message);
            }
        }

     

        private static void FillObjects(List<SolicitacaoViagem> solicitacaoViagems)
        {
            solicitacaoViagems.ForEach(x =>
            {
                x.AprovadorSolicitacaoId = x.AprovadorSolicitacaoId.OrderBy(y => y.Id).Reverse().ToList();
                foreach (var aprovadorSolicitacao in x.AprovadorSolicitacaoId)
                {
                    aprovadorSolicitacao.Aprovador = db.Aprovadores.Where(y => y.Id == aprovadorSolicitacao.AprovadorId).
                    Include(y => y.Empregado).FirstOrDefault();
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
