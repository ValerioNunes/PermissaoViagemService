using PermissaoViagemService.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissaoViagemService.Dal
{
    class PermissaoViagemContext : DbContext
    {
        public PermissaoViagemContext() : base("name=PermissaoViagemContext") { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
        public virtual IDbSet<Transporte> Transportes { get; set; }
        public virtual IDbSet<Status> Status { get; set; }
        public virtual IDbSet<ViajanteSolicitacao> ViajanteSolicitacao { get; set; }
        public virtual IDbSet<AprovadorSolicitacao> AprovadorSolicitacao { get; set; }
        public System.Data.Entity.DbSet<Local> Locals { get; set; }
        public System.Data.Entity.DbSet<Aprovador> Aprovadores { get; set; }
        public System.Data.Entity.DbSet<Empregado> Empregados { get; set; }
        public System.Data.Entity.DbSet<SolicitacaoViagem> SolicitacaoViagems { get; set; }

    }
}
