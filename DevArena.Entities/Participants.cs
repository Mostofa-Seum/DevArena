using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevArena.Entities
{
    [Table("participants")]
    public class Participants
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [StringLength(250)]
        public string name { get; set; } = null!;
        [Required]
        [StringLength(150)]
        public string email { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string password { get; set; } = null!;
        [Required]
        public bool Is_active { get; set; } = true;

        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
    }
}