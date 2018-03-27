using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissaoViagemService.Models
{
    [Table("tblocal")]
    public class Local
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("nome")]
        public String Nome { get; set; }
        [Column("idplace")]
        public String IdPlace { get; set; }
    }
}
