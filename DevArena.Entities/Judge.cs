using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevArena.Entities
{
    [Table("judges")]
    public class Judge
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public int participant_id { get; set; }

        [Required]
        public int contest_id { get; set; }

        [Required]
        public int promoted_by_host_id { get; set; }

        [Required]
        public bool Is_active { get; set; } = true;

        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        [ForeignKey("participant_id")]
        public virtual Participants Participants { get; set; } = null!;

        [ForeignKey("contest_id")]
        public virtual Contests Contest { get; set; } = null!;

        [ForeignKey("promoted_by_host_id")]
        public virtual Host Host { get; set; } = null!;
    }
}