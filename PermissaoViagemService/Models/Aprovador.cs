using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissaoViagemService.Models
{
    [Table("tbaprovadores")]
    public class Aprovador
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("idempregado")]
        public int EmpregadoId { get; set; }
        public Empregado Empregado { get; set; }
    }
}
