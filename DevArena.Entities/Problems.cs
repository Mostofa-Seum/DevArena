using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevArena.Entities
{
    [Table("problems")]
    public class Problems
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int problem_id { get; set; }

        [Required]
        [StringLength(50)]
        public string title { get; set; } = null!;
        [Required]
        public string description { get; set; } = null!;

        [Required]
        public string input_format { get; set; } = null!;

        [Required]
        public string output_format { get; set; } = null!;

        [Required]
        public int judge_id { get; set; }

        [Required]
        public int contest_id { get; set; }

        [ForeignKey("judge_id")]
        public virtual Judge Judge { get; set; } = null!;

        [ForeignKey("contest_id")]
        public virtual Contests Contest { get; set; } = null!;
    }
}