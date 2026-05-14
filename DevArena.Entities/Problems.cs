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
        public int ProblemId { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string InputFormat { get; set; } = null!;

        [Required]
        public string OutputFormat { get; set; } = null!;

        [Required]
        public int JudgeId { get; set; }

        [Required]
        public int ContestId { get; set; }

        [ForeignKey("JudgeId")]
        public virtual Judge Judge { get; set; } = null!;

        [ForeignKey("ContestId")]
        public virtual Contests Contest { get; set; } = null!;
    }
}