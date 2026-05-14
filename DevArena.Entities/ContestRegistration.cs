using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevArena.Entities
{
    [Table("contest_registration")]
    public class ContestRegistration
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public int contest_id { get; set; }

        [Required]
        public int participant_id { get; set; }

        [Required]
        public DateTime reg_time { get; set; } = DateTime.UtcNow;

        [Required]
        public bool is_active { get; set; } = true;


        [ForeignKey("contest_id")]
        public virtual Contests Contest { get; set; }

        [ForeignKey("participant_id")]
        public virtual Participants Participant { get; set; }
    }
}