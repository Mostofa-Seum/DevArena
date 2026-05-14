using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevArena.Entities
{
    [Table("admins")]
    public class Admin
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        [StringLength(250)]
        public string name { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string email { get; set; } = null!;
        [Required]
        [PasswordPropertyText]
        public string password { get; set; } = null!;
        public bool Is_active { get; set; } = true;
    }
}
