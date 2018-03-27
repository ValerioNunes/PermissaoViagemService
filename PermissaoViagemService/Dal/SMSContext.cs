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
    class SMSContext : DbContext
    {
        public SMSContext() : base("name=SMSContext") { }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
        public virtual IDbSet<SmsQueue> SmsQueue { get; set; }
    }
}
